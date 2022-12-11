import json
import random
import time

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

    def __init__(self, Row, Col, Blocks):
        self.Row = Row
        self.Col = Col
        self.blocks = []

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
    Speed = 0
    Exp, Level = 0, 0
    Element = 0
    RelicID = None
    CanMove, CanAttack = True, True
    Faction = None

    # TODO: 射程和移动需要区分一下

    def __init__(self, ID, Character, Faction, Strength, Defence, Life, Range, Speed, Type, Row, Col, RelicID,
                 CanMove, CanAttack, Exp, Level):
        self.ID = ID
        self.Faction = Faction
        self.Character = Character

        self.Strength = Strength
        self.Defence = Defence
        self.Life = Life
        self.Range = Range
        self.Speed = Speed
        self.Type = Type

        self.Row = Row
        self.Col = Col
        self.RelicID = RelicID
        self.CanMove = CanMove
        self.CanAttack = CanAttack

        self.Exp = Exp
        self.Level = Level

    def package(self) -> dict:
        p = {"ID": self.ID, "Character": self.Character, "Row": self.Row, "Col": self.Col,
             "Strength": self.Strength, "Defence": self.Defence, "Life": self.Life,
             "Range": self.Range, "Speed": self.Speed, "Exp": self.Exp, "Level": self.Level, "Element": self.Element,
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
    map = None
    units = {}  # {key: str, value: NetUnit}
    buildings = {}  # {key: str, value: NetBuilding}
    relics = {}  # {key: str, value: NetRelic}
    player = True
    toSend = []
    step = 0

    def __init__(self):
        self.map = None
        self.units = {}  # {key: str, value: NetUnit}
        self.buildings = {}  # {key: str, value: NetBuilding}
        self.relics = {}  # {key: str, value: NetRelic}
        self.player = True
        self.toSend = []
        self.step = 0

    def restart(self, SaveName):  # 初始化 default
        self.map = None
        self.units = {}
        self.buildings = {}
        self.relics = {}
        self.map = {}
        self.player = True
        self.toSend = []
        self.step = 0

        self.handle_read("Saving/" + SaveName)
        self.record_for_rollback()

    def set_map(self, Row, Col, Blocks):
        self.map = NetMap(Row, Col, Blocks)
        self.send(OperationType.ServerSetMap.value, json.dumps(self.map.package()))

    def set_unit(self, units):  # TODO: 读档
        self.units = {}
        for i in units["Units"]:
            a = NetUnit(i["ID"], i["Character"], i["Faction"], i["Strength"], i["Defence"], i["Life"], i["Range"],
                        i["Speed"], i["Type"], i["Row"], i["Col"], i["RelicID"], i["CanMove"], i["CanAttack"],
                        i["Exp"], i["Level"])
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

    def handle_interact(self, From: str, To: str, AI=False):  # 交互
        attacker = self.units[From]
        defender = self.units[To]
        if (attacker.Row - defender.Row) ** 2 + (attacker.Col - defender.Col) ** 2 <= attacker.Range ** 2 \
                and attacker.CanAttack and self.checkFaction(attacker) and not self.checkFaction(defender):
            attacker.CanAttack = False
            if attacker.Strength - defender.Defence > 0:
                defender.Life -= (attacker.Strength - defender.Defence)
            if defender.Life <= 0:
                self.units.pop(defender.ID)
                attacker.Level += defender.Level
                self.LevelUp(attacker, defender.Level)

            self.record_for_rollback()

            if not AI:
                self.send(OperationType.ServerSetUnits.value, self.package_list(self.units, "Units"))
            else:
                self.send(OperationType.ServerSetUnits.value, self.package_list(self.units, "Units"), -1, 1)

            self.JudegeEnd()

    def LevelUp(self, unit, value):
        unit.Strength += value
        unit.Defence += value
        unit.Life += value

    def JudegeEnd(self):
        a = True
        b = True
        for i in self.units:
            if self.units[i].Faction == "Red":
                a = False
            if self.units[i].Faction == "Blue":
                b = False

        if a is True:
            return self.send(OperationType.ServerEndGame.value, True)
        if b is True:
            return self.send(OperationType.ServerEndGame.value, False)

    def handle_move(self, ID: str, Row: int, Col: int, AI=False):
        if (self.units[ID].Row - Row) ** 2 + (self.units[ID].Col - Col) ** 2 <= self.units[ID].Speed ** 2 \
                and self.units[ID].CanMove and self.checkFaction(self.units[ID]):
            self.units[ID].Row = Row
            self.units[ID].Col = Col
            self.units[ID].CanMove = False

            self.record_for_rollback()
            if not AI:
                self.send(OperationType.ServerSetUnits.value, self.package_list(self.units, "Units"))
            else:
                self.send(OperationType.ServerSetUnits.value, self.package_list(self.units, "Units"), -1, 1)

    def handle_assignRelic(self, ID: str, Relic: str):
        if self.units[ID].RelicID is None and self.checkFaction(self.units[ID]):
            self.units[ID].RelicID = Relic
            if Relic == "R0":
                self.assignRelicValue(self.units[ID], RelicType.R0)
            if Relic == "R1":
                self.assignRelicValue(self.units[ID], RelicType.R1)
            if Relic == "R2":
                self.assignRelicValue(self.units[ID], RelicType.R2)

            self.record_for_rollback()

            self.send(OperationType.ServerSetUnits.value, self.package_list(self.units, "Units"))

    def assignRelicValue(self, unit, type):
        unit.Strength += type.value["Strength"]
        unit.Defence += type.value["Defence"]
        unit.Life += type.value["Life"]
        unit.Range += type.value["Range"]
        unit.Speed += type.value["Speed"]

    def handle_addBuilding(self, Type: str, Row: int, Col: int):
        n = len(self.buildings)
        b = NetBuilding('Building' + str(n), Row, Col, Type, Faction.Blue)
        b.Type = Type
        b.ID = 'Building' + str(n)
        self.buildings[b.ID] = b
        self.send(OperationType.ServerSetBuildings.value, self.package_list(self.buildings, "Buildings"))

    def handle_rollBack(self):
        print(self.step)
        if self.step >= 2:
            with open("RollBack/" + str(self.step - 2), 'r') as f:
                read = f.read()
            read = read.split("\n")
            self.step -= 1
            character = json.loads(read[0])
            self.player = bool(read[1])
            self.set_unit(character)

    def handle_endRound(self, AI=False):
        self.player = not self.player  # TODO: 交换阵营
        for i in self.units:
            if self.checkFaction(self.units[i]):
                self.units[i].CanMove = True
                self.units[i].CanAttack = True
            else:
                self.units[i].CanMove = False
                self.units[i].CanAttack = False

        self.record_for_rollback()
        if not AI:
            self.send(OperationType.ServerSetUnits.value, self.package_list(self.units, "Units"))
        else:
            self.send(OperationType.ServerSetUnits.value, self.package_list(self.units, "Units"), -1, 1)

    def record_for_rollback(self):
        with open("RollBack/" + str(self.step), 'w') as f:
            f.write(self.package_list(self.units, "Units"))
            f.write("\n")
            f.write(str(self.player))

        self.step += 1

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

    def AI_Operation(self):
        my = []
        op = []
        for ID in self.units:
            if self.checkFaction(self.units[ID]):
                my.append(self.units[ID])
            else:
                op.append(self.units[ID])
        for u_my in my:
            x = op[0].Row - u_my.Row
            y = op[0].Col - u_my.Row
            move_x = (u_my.Speed * x) // ((x ** 2 + y ** 2) ** 0.5) + u_my.Row
            move_y = (u_my.Speed * y) // ((x ** 2 + y ** 2) ** 0.5) + u_my.Col
            if x < 0:
                move_x += 1
            if y < 0:
                move_y += 1
            if self.move_valid(move_x, move_y):
                self.handle_move(u_my.ID, int(move_x), int(move_y), True)
            defender = self.attack_valid(u_my, op)
            if defender is not None:
                self.handle_interact(u_my.ID, defender.ID, True)

        self.handle_endRound(True)

    def attack_valid(self, my, ops):
        for op in ops:
            if (op.Row - my.Row) ** 2 + (op.Col - my.Col) ** 2 <= my.Range ** 2:
                return op
        return None

    def move_valid(self, row, col):
        for ID in self.units:
            if row == self.units[ID].Row and col == self.units[ID].Col:
                return False
            if row >= 10 or row < 0 or col >= 10 or col < 0:
                return False

        return True

    def send(self, type, value, target=-1, delay=0):
        self.toSend.append((type, value, target, delay))  # -1, 0
        print(type, ">", value, target, delay)

    def clearbuf(self):
        self.toSend = []

    def getbuf(self):
        return self.toSend

    def checkFaction(self, unit):
        if (unit.Faction == "Blue" and self.player) or (unit.Faction == "Red" and not self.player):
            return True
        else:
            return False


if __name__ == '__main__':
    g = Game()
    g.restart("default.txt")
    g.AI_Operation()
    g.AI_Operation()
    g.AI_Operation()





