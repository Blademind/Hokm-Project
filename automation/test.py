from ast import pattern
from linked_list import *
from base_classes import *
import json
import re

s = "client id:5"

print(type(re.findall(string = s, pattern=r":([1-9]{0,})$")[0]))