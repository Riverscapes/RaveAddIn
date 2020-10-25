"""
Constants
    - Here we have a file containing the constants we typically need to run BRAT.
    - These can all be overridden by a .env file
"""
import codecs
import re
import os
from lib import xml_builder
from version import __version__

_INIT_PARAMS = {
    'PROJ_FILE': 'project.rs.xml',
    'OUTPUT_EPSG': 4326
}


class ModelConfig:

    def __init__(self, xsd_url):
        self.env = self.parse_dotenv(os.path.join(os.path.dirname(__file__), '.env'))

        # This XSD is what we use to validate the XML. For writing project XMLs this is placed
        # In the top line. It is not used and only here for reference
        self.XSD_URL = xsd_url
        # Output coordinate system for riverscapes
        # https://en.wikipedia.org/wiki/World_Geodetic_System#WGS84
        # https://spatialreference.org/ref/epsg/4326/
        self.OUTPUT_EPSG = _INIT_PARAMS['OUTPUT_EPSG']

        # The name of the project XML file
        self.PROJ_XML_FILE = _INIT_PARAMS['PROJ_FILE']
        self.version = __version__

        # Anything in the .env file will overwrite these values
        [setattr(self, k, v) for k, v in self.env.items()]

    def parse_dotenv(self, dotenv_path):
        results = {}
        if not os.path.exists(dotenv_path):
            return results
        with open(dotenv_path) as f:
            for line in f:
                line = line.strip()
                if not line or line.startswith('#') or '=' not in line:
                    continue
                k, v = line.split('=', 1)

                # Remove any leading and trailing spaces in key, value
                k, v = k.strip(), v.strip().encode('unicode-escape').decode('ascii')
                if len(v) > 0:
                    quoted = v[0] == v[len(v) - 1] in ['"', "'"]

                    if quoted:
                        v = codecs.getdecoder('unicode_escape')(v[1:-1])[0]
                results[k] = v
        return results

    def parse_args_env(self, parser):
        """substitute environment variables for parameters
        Arguments:
            args {[type]} -- [description]
        """
        args = parser.parse_args()
        pattern = r'{env:([^}]+)}'

        for k, v in vars(args).items():
            if type(v) is str:
                m = re.match(pattern, v)
                if m:
                    envname = m.group(1)
                    if envname not in os.environ and envname not in self.env:
                        raise Exception('COULD NOT FIND ENVIRONMENT VARIABLE: {}'.format(envname))

                    # There is a precedence here:
                    if envname in os.environ:
                        setattr(args, k, re.sub(pattern, os.environ[envname].replace('\\', '/'), v))
                    if envname in self.env:
                        setattr(args, k, re.sub(pattern, self.env[envname].replace('\\', '/'), v))

        return args
