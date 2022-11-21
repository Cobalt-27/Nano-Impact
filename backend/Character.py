from enum import Enum
from Shared import *


class Character(Enum):
    Nahida = {}
    Amber = {"Character": "Amber", "Strength": 10, "Defence": 10, "Life": 10, "Range": 1, "Type": UnitType.Melee}
