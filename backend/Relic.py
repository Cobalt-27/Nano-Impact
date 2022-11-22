from enum import Enum
from Shared import *


class RelicType(Enum):
    R0 = {"RelicType": "R0", "Strength": 10, "Defence": 10, "Life": 10, "Range": 1}
    R1 = {"RelicType": "R1", "Strength": 1, "Defence": 1, "Life": 1, "Range": 10}