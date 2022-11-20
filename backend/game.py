import json
from Shared import *


class NetUnit:
    ID = None
    Character = None
    Type = None
    Row, Col = 0, 0
    Strength, Defence, Life, Range = 0, 0, 0, 0
    Exp, Level = 0, 0
    Element = 0
    RelicID = None
    CanMove, CanAttack = False, False
    Faction = None


class NetRelic:
    ID = None
    Character = None


class NetBuilding:
    ID = None
    Type = None
    Row, Col = 0, 0
    Faction = None


class Game:

    def __init__(self):
        self.units = {}  # {key: str, value: NetUnit}
        self.buildings = {}  # {key: str, value: NetBuilding}
        self.relics = []  # {key: str, value: NetRelic}
        self.player = True  # TODO: 判断阵营

    def restart(self):  # 初始化
        pass

    def handle_upgrade(self, id: str):  # 角色升级
        self.units[id].level += 1
        return OperationType.ServerSetUnits, self.units

    def handle_interact(self, From: str, To: str):  # 交互
        attacker = self.units[From]
        defender = self.units[To]
        if (attacker.Row - defender.Row) ** 2 + (attacker.Col - defender.Col) ** 2 < attacker.range ** 2 \
                and attacker.CanAttack:
            defender.Live -= (attacker.Strength - defender.Defense)
        return OperationType.ServerSetUnits, self.units

    def handle_move(self, ID: str, Row: int, Col: int):
        if (self.units[ID].Row - Row) ** 2 + (self.units[ID].Col - Col) ** 2 < self.units[ID].range ** 2 \
                and self.units[ID].CanMove:
            self.units[ID].Row = Row
            self.units[ID].Col = Col

        return OperationType.ServerSetUnits, self.units

    def handle_assignRelic(self, ID: str, Relic: str):
        self.units[ID].relicID = Relic
        return OperationType.ServerSetUnits, self.units

    def handle_addBuilding(self, Type: BuildingType, Row: int, Col: int):
        n = len(self.buildings)
        b = NetBuilding()
        b.Row = Row
        b.Col = Col
        b.Type = Type
        b.ID = 'Building' + str(n)
        self.buildings[b.ID] = b
        return OperationType.ServerSetBuildings, self.buildings

    def handle_endRound(self):
        self.player = not self.player  # TODO: 交换阵营

    def handle_endGame(self):
        return OperationType.ServerEndGame, True  # TODO: 判断输赢

    def handle_save(self, Name: str):
        # TODO: 存档
        pass

    def handle_quit(self):
        # TODO: 退出
        pass


if __name__ == '__main__':
    unit = NetUnit()
    unit.row = 114
    unit.col = 514
    unit.strength = 1
    print(json.dumps(unit.__dict__))
