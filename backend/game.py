import json
from Shared import *
import random
from Character import *
from Relic import *


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

    def __init__(self, Row, Col, Blocks=None):
        self.Row = Row
        self.Col = Col
        if Blocks is None:
            for i in range(Row):
                r = []
                for j in range(Col):
                    r.append(NetBlock(i, j, random.random(), random.randint(0, 4)))
                self.blocks.append(r)
        else:
            for i in range(Row):
                r = []
                for j in range(Col):
                    r.append(
                        NetBlock(Blocks[i * Col + j]["Row"], Blocks[i * Col + j]["Col"], Blocks[i * Col + j]["Height"],
                                 Blocks[i * Col + j]["Type"]))
                self.blocks.append(r)

    def package(self) -> dict:
        m = {"Row": self.Row, "Col": self.Col, "Blocks": []}
        for i in range(len(self.blocks)):
            for j in range(len(self.blocks[0])):
                b = self.blocks[i][j].__dict__
                m["Blocks"].append(b)
        return m


class NetUnit:
    ID = None  # TODO:下划线开头
    Character = None
    Type = None
    Row, Col = 0, 0
    Strength, Defence, Life, Range = 0, 0, 0, 0
    Exp, Level = 0, 0
    Element = 0
    RelicID = None
    CanMove, CanAttack = True, True
    Faction = None

    # TODO: 射程和移动需要区分一下

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
    Type = None

    # TODO: 圣遗物的属性如何定义

    def __init__(self, ID, Type):
        self.ID = ID
        self.Type = Type

    def package(self) -> dict:
        p = {"ID": self.ID, "RelicType": self.Type.value["RelicType"]}
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
        self.map = None
        self.units = {}  # {key: str, value: NetUnit}
        self.buildings = {}  # {key: str, value: NetBuilding}
        self.relics = {}  # {key: str, value: NetRelic}
        self.player = True  # TODO: 判断阵营

    def restart(self, MapRow, MapCol, Seed, Load, SaveName):  # 初始化
        self.units = {}
        self.buildings = {}
        self.relics = {}
        self.player = True

        if Load is False:

            self.map = NetMap(MapRow, MapCol)
            self.units['C1'] = NetUnit('C1', Character.Amber, Faction.Friendly)
            self.units['C2'] = NetUnit('C2', Character.Nahida, Faction.Friendly)
            self.buildings['B1'] = NetBuilding('B1', BuildingType.Church, Faction.Friendly)
            self.buildings['B2'] = NetBuilding('B2', BuildingType.Statue, Faction.Friendly)
            self.relics['R1'] = NetRelic('R1', RelicType.R0)
            self.relics['R2'] = NetRelic('R2', RelicType.R1)

            send(OperationType.ServerSetMap.value, json.dumps(self.map.package()))
            send(OperationType.ServerSetUnits.value, self.package_list(self.units, "Units"))
            send(OperationType.ServerSetBuildings.value, self.package_list(self.buildings, "Buildings"))
            send(OperationType.ServerSetRelics.value, self.package_list(self.relics, "Relics"))

        else:
            self.handle_read("Saving/" + SaveName)

    def set_map(self, Row, Col, Blocks):
        self.map = NetMap(Row, Col, Blocks)
        send(OperationType.ServerSetMap.value, self.map.package())

    def set_unit(self, units):  # TODO: 读档
        pass

    def set_building(self, buildings):
        pass

    def set_relic(self, relics):
        pass

    def handle_upgrade(self, id: str):  # 角色升级
        self.units[id].Level += 1
        send(OperationType.ServerSetUnits.value, self.package_list(self.units, "Units"))

    def handle_interact(self, From: str, To: str):  # 交互
        attacker = self.units[From]
        defender = self.units[To]
        if (attacker.Row - defender.Row) ** 2 + (attacker.Col - defender.Col) ** 2 < attacker.Range ** 2 \
                and attacker.CanAttack:
            attacker.CanAttack = False
            if attacker.Strength - defender.Defence > 0:
                defender.Life -= (attacker.Strength - defender.Defence)

        send(OperationType.ServerSetUnits.value, self.package_list(self.units, "Units"))

        for i in self.units:
            if self.units[i].Faction == Faction.Hostile:
                return
        send(OperationType.ServerEndGame.value, True)

    def handle_move(self, ID: str, Row: int, Col: int):
        if (self.units[ID].Row - Row) ** 2 + (self.units[ID].Col - Col) ** 2 < self.units[ID].Range ** 2 \
                and self.units[ID].CanMove:
            self.units[ID].Row = Row
            self.units[ID].Col = Col
            self.units[ID].CanMove = False

        send(OperationType.ServerSetUnits.value, self.package_list(self.units, "Units"))

    def handle_assignRelic(self, ID: str, Relic: str):
        self.units[ID].relicID = Relic
        send(OperationType.ServerSetUnits.value, self.package_list(self.units, "Units"))

    def handle_addBuilding(self, Type: BuildingType, Row: int, Col: int):
        n = len(self.buildings)
        b = NetBuilding('Building' + str(n), Type, Faction.Friendly)
        b.Row = Row
        b.Col = Col
        b.Type = Type
        b.ID = 'Building' + str(n)
        self.buildings[b.ID] = b
        send(OperationType.ServerSetBuildings.value, self.package_list(self.buildings, "Buildings"))

    def handle_endRound(self):
        self.player = not self.player  # TODO: 交换阵营

    def handle_save(self, Name: str):
        with open("Saving/" + Name, 'w') as f:
            f.write(json.dumps(self.map.package()))
            f.write("\n")
            f.write(self.package_list(self.units, "Units"))
            f.write("\n")
            f.write(self.package_list(self.buildings, "Buildings"))
            f.write("\n")
            f.write(self.package_list(self.relics, "Relics"))
            f.write("\n")
            f.write(str(self.player))

    def handle_read(self, Name: str):
        with open("Saving/" + Name, 'r') as f:
            read = f.read()
        read = read.split("\n")
        map = json.loads(read[0])
        character = json.loads(read[1])
        building = json.loads(read[2])
        relic = json.loads(read[3])

        self.set_map(map["Row"], map["Col"], map["Blocks"])
        self.set_unit(character)
        self.set_building(building)
        self.set_relic(relic)

    def handle_quit(self):

        send(OperationType.ServerEndGame.value, False)

    def package_list(self, data, type=None):
        d = []
        for i in data:
            d.append(data[i].package())
        if type is not None:
            s = json.dumps({type: d})
        else:
            s = json.dumps(d)
        return s


def send(type: str, data):
    print('>', type, data)


if __name__ == '__main__':
    map = NetMap(10, 10)

    g = Game()
    g.restart(10, 10, None, False, None)

    g.handle_upgrade("C1")
    g.handle_save("s1.txt")
    g.handle_read("s1.txt")
    g.handle_interact("C1", "C2")
