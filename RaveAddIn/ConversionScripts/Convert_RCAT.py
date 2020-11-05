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
cfg = ModelConfig('http://xml.riverscapes.xyz/Projects/XSD/V1/RCA.xsd')


def build_xml(RCA_folder):

    in_dict, int_dict, out_dict = get_RCA_variables(RCA_folder)
    in_layers_dict, int_layers_dict, out_layers_dict = generate_layers_dict(in_dict, int_dict, out_dict)
    huc_num, huc_name = get_huc_data(RCA_folder)

    # Create the top-level nodes
    log = Logger('build_xml')
    log.info('Starting the build of the XML')
    project_name = "RCA for HUC {}".format(huc_num)
    project = RSProject(cfg, RCA_folder)
    project.create(project_name, 'RCA')

    # Add the root metadata
    project.add_metadata({
        'ModelVersion': cfg.version,
        'HUC8': huc_num,
        #TODO once we know RCA version, put it here
        'RCA Version': cfg.version,
        # TODO add the HUC name here
        'watershed': huc_name,
    })

    # Create the realizations container node
    realizations = project.XMLBuilder.add_sub_element(project.XMLBuilder.root, 'Realizations')

    # Example InundationContext Realization
    # ================================================================================================
    r1_node = project.XMLBuilder.add_sub_element(realizations, 'RCA', None, {
        'id': 'RCA_1',
        'dateCreated': datetime.datetime.now().isoformat(),
        'guid': str(uuid.uuid1()),
        'productVersion': cfg.version
    })
    #  add a <Name> node
    project.XMLBuilder.add_sub_element(r1_node, 'Name', project_name)

    # Add an <Input> and <Output> nodes
    r1_inputs = project.XMLBuilder.add_sub_element(r1_node, 'Inputs')
    rl_intermediates = project.XMLBuilder.add_sub_element(r1_node, 'Intermediates')
    r1_outputs = project.XMLBuilder.add_sub_element(r1_node, 'Outputs')

    # Now we can add inputs to the context raster
    for key in in_layers_dict.keys():
        if is_layer_raster(in_layers_dict[key]):
            project.add_project_raster(r1_inputs, in_layers_dict[key], replace=True)
        else:
            project.add_project_vector(r1_inputs, in_layers_dict[key], replace=True)

    for key in int_layers_dict.keys():
        if is_layer_raster(int_layers_dict[key]):
            project.add_project_raster(rl_intermediates, int_layers_dict[key], replace=True)
        else:
            project.add_project_vector(rl_intermediates, int_layers_dict[key], replace=True)

    for key in out_layers_dict.keys():
        if is_layer_raster(out_layers_dict[key]):
            project.add_project_raster(r1_outputs, out_layers_dict[key], replace=True)
        else:
            project.add_project_vector(r1_outputs, out_layers_dict[key], replace=True)





    # Finally write the file
    log.info('Writing file')
    project.XMLBuilder.write()
    log.info('Done')


def get_RCA_variables(RCA_folder):

    in_dict = {}
    int_dict = {}
    out_dict = {}
    all_files = get_all_files(RCA_folder)

    in_dict['NHD Flowlines'] = search_for_key(all_files, ["NHD", "Reaches", "Stream", "Seg", "Segment"])
    in_dict['Existing Vegetation'] = search_for_key(all_files, ["lf_evt", "EVT", "Existing"], ['Cover', 'Vegetated', 'Native', 'Riparian'])
    in_dict['Historic Vegetation'] = search_for_key(all_files, ["lf_his", "BPS", "Historic"], ['Cover', 'Vegetated', 'Native', 'Riparian'])
    in_dict['Fragmented Valley Bottom'] = search_for_key(all_files, ["VB_01_Both", "VB_Frag_02", "Frag", "Valley", "VB", "vb"])
    in_dict['Large River Polygon'] = search_for_key(all_files, ["LRP", "LargeRiverPolygon"])
    in_dict['NED 10m DEM'] = search_for_key(all_files, ["DEM", "10m", "dem"])
    in_dict['Precipitation'] = search_for_key(all_files, ["Precip", "precip", "prec", "Prec"])
    in_dict['Flow Accumulation'] = search_for_key(all_files, ["DrainArea_sqkm", "DrainArea", "accum", "Accum"])

    int_dict['Thiessen Midpoints'] = search_for_key(all_files, ["midpoints", "mid", "thiessen"])
    int_dict['Clipped Thiessen Polygons'] = search_for_key(all_files, ["Thiessen_Valley_Clip", "thiessen", "Thiessen"])
    int_dict['Unclipped Thiessen Polygons'] = search_for_key(all_files, ["thiessen", "Thiessen"], ["Clip", "clip"])

    int_dict['Existing Cover'] = search_for_key(all_files, ["Ex_Cover", "Cover"], ["Hist", "his", "BPS", 'bps'])
    int_dict['Existing Vegetated'] = search_for_key(all_files, ["Existing_Vegetated", "Ex_Vegetated", "Vegetated"], ["His", "his", "BPS", 'bps'])
    int_dict['Existing Native Riparian'] = search_for_key(all_files, ["Existing_Native", "Ex_Native", "Native"], ["His", "his", "BPS", 'bps'])
    int_dict['Existing Riparian'] = search_for_key(all_files, ["Existing_Riparian", "Ex_Riparian", "Native"], ["His", "his", "BPS", 'bps', "Native"])

    int_dict['Historic Cover'] = search_for_key(all_files, ["Hist_Cover", "Cover"], ['Ex', "ex", "EVT", "evt"])
    int_dict['Historic Vegetated'] = search_for_key(all_files, ["Hist_Vegetated", "Historic_Vegetated", "Vegetated"], ['Ex', "ex", "EVT", "evt"])
    int_dict['Historic Native Riparian'] = search_for_key(all_files, ["Historic_Native", "Hist_Native", "Native"], ['Ex', "ex", "EVT", "evt"])
    int_dict['Historic Riparian'] = search_for_key(all_files, ["Historic_Riparian", "Hist_Riparian", "Native"], ['Ex', "ex", "EVT", "evt", "Native"])

    int_dict['Riparian Corridor'] = search_for_key(all_files, ["All_Riparian", "Riparian"], ['Ex', "ex", "EVT", "evt", "His", "his", "BPS", 'bps'])
    int_dict['Conversion Raster'] = search_for_key(all_files, ["Converstion_Raster", "Conversion"], ['Ex', "ex", "EVT", "evt", "His", "his", "BPS", 'bps'])
    int_dict['Land Use Intensity'] = search_for_key(all_files, ["Land_Use", "landuse", "Intensity"])

    int_dict['Bankfull Merged'] = search_for_key(all_files, ["bankfull"], ['dissolve', 'merge'])
    int_dict['Minimum Buffer'] = search_for_key(all_files, ["min_buffer", "buffer"])

    int_dict['Bankfull Channel Width'] = search_for_key(all_files, ["Conf", "Conf_Thiessen_Bankfull"], ["Valley", "valley"])
    int_dict['Valley Bottom Width'] = search_for_key(all_files, ["Conf", "Conf_Thiessen_Valley"], ["Bankfull", "bankfull"])

    int_dict['Floodplain Connectivity'] = search_for_key(all_files, ["Floodplain_Connectivity", "Connectivity"])

    out_dict['Riparian Vegetation Departure'] = search_for_key(all_files, ["RVD"])

    out_dict['Bankfull Channell Polygon'] = search_for_key(all_files, ["BankfullChannelPolygon"])
    out_dict['Bankfull Widths Network'] = search_for_key(all_files, ["BankfullWidthsNetwork"])

    out_dict['Confinement Network'] = search_for_key(all_files, ["ConfinementNetwork"])

    out_dict['Riparian Condition Assesment'] = search_for_key(all_files, ["_RCA", "RCA"])

    return in_dict, int_dict, out_dict


def generate_layers_dict(in_dict, int_dict, out_dict):
    in_layers = {}
    int_layers = {}
    out_layers = {}

    for key in in_dict.keys():
        in_layers[key] = RSLayer(key, sanitize_name(key), None, in_dict[key])
    for key in int_dict.keys():
        int_layers[key] = RSLayer(key, sanitize_name(key), None, int_dict[key])
    for key in out_dict.keys():
        out_layers[key] = RSLayer(key, sanitize_name(key), None, out_dict[key])

    return in_layers, int_layers, out_layers


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

    return "COULD NOT FIND {}".format(keys)


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
    parser.add_argument('projectpath', help='RCA Project Folder', type=str)
    parser.add_argument('--verbose', help='(optional) a little extra logging ', action='store_true', default=False)

    args = cfg.parse_args_env(parser)

    if args.projectpath is None or len(args.projectpath) < 10:
        raise Exception('projectpath has invalid value')
    safe_makedirs(args.projectpath)
    # Initiate the log file
    log = Logger('RCA XML')
    log.setup(logPath=os.path.join(args.projectpath, 'RCA.log'), verbose=args.verbose)

    try:
        log.info('Starting')
        build_xml(args.projectpath)
        log.info('Exiting')

    except Exception as e:
        log.error(e)
        traceback.print_exc(file=sys.stdout)
        sys.exit(1)

    sys.exit(0)
