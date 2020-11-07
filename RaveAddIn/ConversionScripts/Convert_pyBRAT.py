from settings import ModelConfig
from lib.util import safe_makedirs
from lib.loghelper import Logger
import os
import sys
import datetime
import traceback
import time
import argparse
import uuid
import re
from lib.project import RSProject, RSLayer

"""

Instructuions:

    1. create a file called .env in the root and add the following line (without the quotes):
                "PROJECTFILE=C:/path/to/my/folder/that/will/have/project.rs.xml"
    2. Run `BuildXML` from VSCode's Run dropdown menu


"""

# change to this value after the pull request is finished:
#TODO ask Matt for help with this one
cfg = ModelConfig('http://xml.riverscapes.xyz/Projects/XSD/V1/pyBRAT.xsd')

# Define the types of layers we're going to use up top so we can re-use them later
#TODO not sure what this is



def build_xml(brat_folder):

    in_dict, out_dict = get_brat_variables(brat_folder)
    in_layers_dict, out_layers_dict = generate_layers_dict(in_dict, out_dict)
    huc_num, huc_name = get_huc_data(brat_folder)

    # Create the top-level nodes
    log = Logger('build_xml')
    log.info('Starting the build of the XML')
    project_name = "pyBRAT for HUC {}".format(huc_num)
    project = RSProject(cfg, brat_folder)
    project.create(project_name, 'pyBRAT')

    # Add the root metadata
    project.add_metadata({
        'ModelVersion': cfg.version,
        'HUC8': huc_num,
        #TODO once we know pyBRAT version, put it here
        'pyBRAT Version': cfg.version,
        # TODO add the HUC name here
        'watershed': huc_name,
    })

    # Create the realizations container node
    realizations = project.XMLBuilder.add_sub_element(project.XMLBuilder.root, 'Realizations')

    # Example InundationContext Realization
    # ================================================================================================
    r1_node = project.XMLBuilder.add_sub_element(realizations, 'pyBRAT', None, {
        'id': 'pyBRAT_1',
        'dateCreated': datetime.datetime.now().isoformat(),
        'guid': str(uuid.uuid1()),
        'productVersion': cfg.version
    })
    #  add a <Name> node
    project.XMLBuilder.add_sub_element(r1_node, 'Name', project_name)

    # Add an <Input> and <Output> nodes
    r1_inputs = project.XMLBuilder.add_sub_element(r1_node, 'Inputs')
    r1_outputs = project.XMLBuilder.add_sub_element(r1_node, 'Outputs')

    # Now we can add inputs to the context raster
    for key in in_layers_dict.keys():
        if is_layer_raster(in_layers_dict[key]):
            project.add_project_raster(r1_inputs, in_layers_dict[key], replace=True)
        else:
            project.add_project_vector(r1_inputs, in_layers_dict[key], replace=True)

    for key in out_layers_dict.keys():
        if is_layer_raster(out_layers_dict[key]):
            project.add_project_raster(r1_outputs, out_layers_dict[key], replace=True)
        else:
            project.add_project_vector(r1_outputs, out_layers_dict[key], replace=True)





    # Finally write the file
    log.info('Writing file')
    project.XMLBuilder.write()
    log.info('Done')


def get_brat_variables(brat_folder):

    in_dict = {}
    out_dict ={}
    all_files = get_all_files(brat_folder)

    #huc_number =
    #huc_name =
    #pybrat_version =


    in_dict['Existing Vegetation'] = search_for_key(all_files, ["EVT", "Existing"])
    in_dict['Historic Vegetation'] = search_for_key(all_files, ["BPS", "Historic"])
    in_dict['NHD Flowlines'] = search_for_key(all_files, ["NHD", "Reaches"])
    in_dict['NED 10m DEM'] = search_for_key(all_files, ["DEM", "10m"])
    in_dict['Drainage Area in sqkm'] = search_for_key(all_files, ["DrainArea"])
    in_dict['DEM Hillshade'] = search_for_key(all_files, ["Hillshade"])
    in_dict['Slope Raster'] = search_for_key(all_files, ["Slope"])
    in_dict['Valley Bottom'] = search_for_key(all_files, ["ValleyBottom", "Provisional"])
    in_dict['Roads'] = search_for_key(all_files, ["roads", "Roads"])
    in_dict['Rails'] = search_for_key(all_files, ["rails", "Rails"])
    in_dict['Canals'] = search_for_key(all_files, ["Canals"])
    in_dict['Points of Diversion'] = search_for_key(all_files, ["diversion"])
    in_dict['Land Use Raster'] = search_for_key(all_files, ["land_use", "landuse", "EVT", "Existing"])
    in_dict['land Ownership Raster'] = search_for_key(all_files, ["Ownership", "SurfaceManagement", "Agency"])
    in_dict['NHD Flowlines (Perennial)'] = search_for_key(all_files, ["NHD_24k_perennial", "Perennial"])
    in_dict['Beaver Dams'] = search_for_key(all_files, ["Dams", "BeaverDams", "Beaver_Dams"])
    out_dict['Data Validation'] = search_for_key(all_files, ["Data_Validation", "Validation"], ["Perennial"])
    out_dict['Data Validation (Perennial)'] = search_for_key(all_files, ["Data_Validation_Perennial"])
    out_dict['BRAT Table'] = search_for_key(all_files, ["BRAT_Table", "BRAT"], ["Perennial"])
    out_dict['BRAT Table (Perennial)'] = search_for_key(all_files, ["BRAT_Table_Perennial"])
    out_dict['Combined Capacity'] = search_for_key(all_files, ["Combined_Capacity", "Capacity"], ["Perennial"])
    out_dict['Combined Capacity (Perennial)'] = search_for_key(all_files, ["Combined_Capacity_Perennial"])
    out_dict['Conservation Restoration'] = search_for_key(all_files, ["Conservation_Restoration"], ["Perennial"])
    out_dict['Conservation Restoration (Perennial)'] = search_for_key(all_files, ["Conservation_Restoration_Perennial"])

    return in_dict, out_dict


def generate_layers_dict(in_dict, out_dict):
    in_layers = {}
    out_layers = {}

    for key in in_dict.keys():
        if not in_dict[key] == 'COULD NOT FIND':
            in_layers[key] = RSLayer(key, sanitize_name(key), None, in_dict[key])
    for key in out_dict.keys():
        if not out_dict[key] == 'COULD NOT FIND':
            out_layers[key] = RSLayer(key, sanitize_name(key), None, out_dict[key])

    return in_layers, out_layers


def sanitize_name(name):
    return name.replace(" ", "_").replace("(","").replace(")","").upper()

def get_all_files(root):

    to_return = []

    for path, subdirs, files in os.walk(root):
        for name in files:
            if name.endswith('.tif') or name.endswith('.shp'):
                to_return.append(os.path.join(path, name))

    return to_return


def search_for_key(all_files, keys, excludes=[]):

    stripped_files = []
    for file in all_files:
        stripped_files.append(file.split("\\")[-1])

    for key in keys:

        to_return = [s for s in stripped_files if key in s]

        for exclude in excludes:
            bad_list = [s for s in stripped_files if exclude in s]
            for bad_item in bad_list:
                while bad_item in to_return:
                    to_return.remove(bad_item)

        if len(to_return) > 0:
            return all_files[stripped_files.index(to_return[0])]

    return "COULD NOT FIND"


def is_layer_raster(layer):
    return layer.rel_path.endswith(".tif")


def get_huc_data(project_folder):
    this_folder = project_folder.split('\\')[-1]
    huc8 = this_folder.split('_')[-1]
    huc_name = this_folder.replace("_{}".format(huc8), "")

    # Add spaces
    huc_name = re.sub('([A-Z])', r' \1', huc_name)[1:]

    return huc8, huc_name



if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('projectpath', help='pyBRAT Project Folder', type=str)
    parser.add_argument('--verbose', help='(optional) a little extra logging ', action='store_true', default=False)

    args = cfg.parse_args_env(parser)

    if args.projectpath is None or len(args.projectpath) < 10:
        raise Exception('projectpath has invalid value')
    safe_makedirs(args.projectpath)
    # Initiate the log file
    log = Logger('pyBRAT XML')
    log.setup(logPath=os.path.join(args.projectpath, 'pyBRAT.log'), verbose=args.verbose)

    try:
        log.info('Starting')
        build_xml(args.projectpath)
        log.info('Exiting')

    except Exception as e:
        log.error(e)
        traceback.print_exc(file=sys.stdout)
        sys.exit(1)

    sys.exit(0)
