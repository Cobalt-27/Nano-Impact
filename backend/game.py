import json
from Shared import *
import random
from Character import *


class NetBlock:
    Row, Col = 0, 0
    Height = 0.0
    Type = None

    def __init__(self, Row, Col, Height, Type):
        self.Row = Row
        self.Col = Col
        self.Height = Height
        self.Type = Type


class NetMap:
    Row, Col = 0, 0
    blocks = []

    def __init__(self, Row, Col):
        self.Row = Row
        self.Col = Col
        for i in range(Row):
            r = []
            for j in range(Col):
                r.append(NetBlock(i, j, random.random(), random.randint(0, 4)))
            self.blocks.append(r)

    def package(self) -> dict:
        m = {"Row": self.Row, "Col": self.Col, "Blocks": []}
        for i in range(len(self.blocks)):
            for j in range(len(self.blocks[0])):
                b = self.blocks[i][j].__dict__
                m["Blocks"].append(b)
        return m


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

    def __init__(self, ID, Character, Faction):
        self.ID = ID
        self.Faction = Faction
        self.Character = Character
        self.Strength, self.Defence, self.Life, self.Range = \
            Character.value["Strength"], Character.value["Defence"], Character.value["Life"], Character.value["Range"]
        self.Type = Character.value["Type"]

    def package(self) -> dict:
        p = {"ID": self.ID, "Character": self.Character.value["Character"], "Row": self.Row, "Col": self.Col,
             "Strength": self.Strength, "Defence": self.Defence, "Life": self.Life,
             "Range": self.Range, "Exp": self.Exp, "Level": self.Level, "Element": self.Element,
             "RelicID": self.RelicID, "CanMove": self.CanMove, "CanAttack": self.CanAttack,
             "Faction": self.Faction.value}

        return p


class NetRelic:
    ID = None
    Character = None

    # TODO: 圣遗物的属性如何定义

    def __init__(self, ID, Type):
        self.ID = ID
        self.Type = Type

    def package(self) -> dict:
        p = {"ID": self.ID, "RelicType": self.Type["RelicType"]}
        return p


class NetBuilding:
    ID = None
    Type = None
    Row, Col = 0, 0
    Faction = None

    def __init__(self, ID, Type, Faction):
        self.ID = ID
        self.Type = Type
        self.Faction = Faction

    def package(self) -> dict:
        p = {"ID": self.ID, "Row": self.Row, "Col": self.Col, "Type": self.Type.value, "Faction": self.Faction.value}
        return p


class Game:

    def __init__(self):
        self.units = {}  # {key: str, value: NetUnit}
        self.buildings = {}  # {key: str, value: NetBuilding}
        self.relics = {}  # {key: str, value: NetRelic}
        self.player = True  # TODO: 判断阵营
        self.map = []

    def restart(self):  # 初始化
        self.units = {}
        self.buildings = {}
        self.relics = {}
        self.player = True

        self.map = NetMap(10, 10)
        self.units['C1'] = NetUnit('C1', Character.Amber, Faction.Friendly)

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

    def package_list(self, data, type):
        d = []
        for i in data:
            d.append(data[i].package())
        s = {type: d}
        return s


async def send(ws, type: str, data: str):
    print('>', type, data)
    await ws.send(f'{type}@{data}')


if __name__ == '__main__':
    map = NetMap(10, 10)
    print(map.package())

    g = Game()
    g.restart()
    print(g.package_list(g.units, "Units"))
