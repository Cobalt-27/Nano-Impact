from enum import Enum
from Shared import *


class Character(Enum):
    Skadi = {"Character": "Skadi", "Strength": 1, "Defence": 1, "Life": 1, "Range": 5, "Type": UnitType.Heal}
    Amiya = {"Character": "Amiya", "Strength": 10, "Defence": 10, "Life": 10, "Range": 1, "Type": UnitType.Melee}
