import json
import random
from Shared import *
from main import send


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

    def __init__(self, ID, Character, Faction, Strength, Defence, Life, Range, Type, Row, Col):
        self.ID = ID
        self.Faction = Faction
        self.Character = Character

        self.Strength = Strength
        self.Defence = Defence
        self.Life = Life
        self.Range = Range
        self.Type = Type

        self.Row = Row
        self.Col = Col

    def package(self) -> dict:
        p = {"ID": self.ID, "Character": self.Character, "Row": self.Row, "Col": self.Col,
             "Strength": self.Strength, "Defence": self.Defence, "Life": self.Life,
             "Range": self.Range, "Exp": self.Exp, "Level": self.Level, "Element": self.Element,
             "RelicID": self.RelicID, "CanMove": self.CanMove, "CanAttack": self.CanAttack,
             "Faction": self.Faction, "Type": self.Type}

        return p


class NetRelic:
    ID = None
    Type = None

    # TODO: 圣遗物的属性如何定义

    def __init__(self, ID, Type):
        self.ID = ID
        self.Type = Type

    def package(self) -> dict:
        p = {"ID": self.ID, "RelicType": self.Type}
        return p


class NetBuilding:
    ID = None
    Type = None
    Row, Col = 0, 0
    Faction = None

    def __init__(self, ID, Row, Col, Type, Faction):
        self.ID = ID
        self.Row = Row
        self.Col = Col
        self.Type = Type
        self.Faction = Faction

    def package(self) -> dict:
        p = {"ID": self.ID, "Row": self.Row, "Col": self.Col, "Type": self.Type, "Faction": self.Faction}
        return p


class Game:

    def __init__(self, send):
        self.map = None
        self.units = {}  # {key: str, value: NetUnit}
        self.buildings = {}  # {key: str, value: NetBuilding}
        self.relics = {}  # {key: str, value: NetRelic}
        self.player = True  # TODO: 判断阵营
        self.send = send

    def restart(self, SaveName):  # 初始化 default
        self.units = {}
        self.buildings = {}
        self.relics = {}
        self.player = True

        self.handle_read("Saving/" + SaveName)

    def set_map(self, Row, Col, Blocks):
        self.map = NetMap(Row, Col, Blocks)
        self.send(OperationType.ServerSetMap.value, json.dumps(self.map.package()))

    def set_unit(self, units):  # TODO: 读档
        self.units = {}
        for i in units["Units"]:
            a = NetUnit(i["ID"], i["Character"], i["Faction"], i["Strength"], i["Defence"], i["Life"], i["Range"],
                        i["Type"], i["Row"], i["Col"])
            self.units[a.ID] = a
        self.send(OperationType.ServerSetUnits.value, self.package_list(self.units, "Units"))

    def set_building(self, buildings):
        self.buildings = {}
        for i in buildings["Buildings"]:
            b = NetBuilding(i["ID"], i["Row"], i["Col"], i["Type"], i["Faction"])
            self.buildings[b.ID] = b
        self.send(OperationType.ServerSetBuildings.value, self.package_list(self.buildings, "Buildings"))

    def set_relic(self, relics):
        self.relics = {}
        for i in relics["Relics"]:
            b = NetRelic(i["ID"], i["RelicType"])
            self.relics[b.ID] = b
        self.send(OperationType.ServerSetRelics.value, self.package_list(self.relics, "Relics"))

    def handle_upgrade(self, id: str):  # 角色升级 ID不存在
        self.units[id].Level += 1
        self.send(OperationType.ServerSetUnits.value, self.package_list(self.units, "Units"))

    def handle_interact(self, From: str, To: str):  # 交互
        attacker = self.units[From]
        defender = self.units[To]
        if (attacker.Row - defender.Row) ** 2 + (attacker.Col - defender.Col) ** 2 <= attacker.Range ** 2 \
                and attacker.CanAttack and attacker.Faction == "Friendly" and defender.Faction == "Hostile":
            attacker.CanAttack = False
            if attacker.Strength - defender.Defence > 0:
                defender.Life -= (attacker.Strength - defender.Defence)
            if defender.Life <= 0:
                self.units.pop(defender.ID)

        self.send(OperationType.ServerSetUnits.value, self.package_list(self.units, "Units"))

        for i in self.units:
            if self.units[i].Faction == "Hostile":
                return
        self.send(OperationType.ServerEndGame.value, True)

    def handle_move(self, ID: str, Row: int, Col: int):
        if (self.units[ID].Row - Row) ** 2 + (self.units[ID].Col - Col) ** 2 <= self.units[ID].Range ** 2 \
                and self.units[ID].CanMove:
            self.units[ID].Row = Row
            self.units[ID].Col = Col
            self.units[ID].CanMove = False

        self.send(OperationType.ServerSetUnits.value, self.package_list(self.units, "Units"))

    def handle_assignRelic(self, ID: str, Relic: str):
        self.units[ID].relicID = Relic
        self.send(OperationType.ServerSetUnits.value, self.package_list(self.units, "Units"))

    def handle_addBuilding(self, Type: str, Row: int, Col: int):
        n = len(self.buildings)
        b = NetBuilding('Building' + str(n), Row, Col, Type, Faction.Friendly)
        b.Type = Type
        b.ID = 'Building' + str(n)
        self.buildings[b.ID] = b
        self.send(OperationType.ServerSetBuildings.value, self.package_list(self.buildings, "Buildings"))

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
        with open(Name, 'r') as f:
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

        self.send(OperationType.ServerEndGame.value, False)

    def package_list(self, data, type=None):
        d = []
        for i in data:
            d.append(data[i].package())
        if type is not None:
            s = json.dumps({type: d})
        else:
            s = json.dumps(d)
        return s


# def send(type: str, data):
#     print('>', type, data)


if __name__ == '__main__':
    g = Game(send)
    g.restart("default.txt")
    g.handle_interact("_1", "_2")
    g.handle_move("_1", 3, 3)

