import sys
import time
import gc
import sys
import glob
import shutil
import os
from math import cos, sin, asin, sqrt, radians
from lib.loghelper import Logger

# Set if this environment variable is set don't show any UI
NO_UI = os.environ.get('NO_UI') is not None


def safe_makedirs(dir_create_path):
    """safely, recursively make a directory

    Arguments:
        dir_create_path {[type]} -- [description]
    """
    log = Logger("MakeDir")

    # Safety check on path lengths
    if len(dir_create_path) < 5 or len(dir_create_path.split('\\')) <= 2:
        raise Exception('Invalid path: {}'.format(dir_create_path))

    if os.path.exists(dir_create_path) and os.path.isfile(dir_create_path):
        raise Exception('Can\'t create directory if there is a file of the same name: {}'.format(dir_create_path))

    if not os.path.exists(dir_create_path):
        try:
            log.info('Folder not found. Creating: {}'.format(dir_create_path))
            os.makedirs(dir_create_path)
        except Exception as e:
            # Possible that something else made the folder while we were trying
            if not os.path.exists(dir_create_path):
                log.error('Could not create folder: {}'.format(dir_create_path))
                raise e
