from enum import Enum


class OperationType(Enum):
    ServerEndGame = 'ServerEndGame'
    ServerSetMap = 'ServerSetMap'
    ServerSetUnits = 'ServerSetUnits'
    ServerSetRelics = 'ServerSetRelics'
    ServerSetBuildings = 'ServerSetBuildings'
    ClientPrint = 'ClientPrint'


class BuildingType(Enum):
    Statue = 'Statue'
    Church = 'Church'


class UnitType(Enum):
    Melee = 'Melee'
    Range = 'Range'
    Heal = 'Heal'


class Faction(Enum):
    Blue = 'Blue'
    Neutral = 'Neutral'
    Red = 'Red'

