from enum import Enum


class OperationType(Enum):
    ServerEndGame = 'ServerEndGame'
    ServerSetMap = 'ServerSetMap'
    ServerSetUnits = 'ServerSetUnits'
    ServerSetRelics = 'ServerSetRelics'
    ServerSetBuildings = 'ServerSetBuildings'
    ClientPrint = 'ClientPrint'


class Character(Enum):
    Nahida = 'Nahida'
    Amber = 'Amber'


class BuildingType(Enum):
    Statue = 'Statue'
    Church = 'Church'


class UnitType(Enum):
    Melee = 'Melee'
    Range = 'Range'
    Heal = 'Heal'

