import json


class NetUnit:

    row = 0
    col = 0
    strength = 1
    defence = 0
    life = 0
    range = 0
    exp = 0
    element = 0
    level = 0
    # def __init__(self):
    #     self.row = 0
    #     self.col = 0
    #     self.strength
    # int row, col, strength, defence, life, range, exp, element, level;
    # public string ID;
    # public Character Character;
    # public UnitType Type;
    # public int Row, Col;
    # public int Strengh, Defence, Life, Range;
    # public int Exp;
    # public bool CanMove, CanAttack;
    # public string RelicID;
    # public Faction Faction;
    pass


class Game:

    def __init__(self):
        self.unit = []

    def start(self):
        pass

    def handle_upgrade(self, id: str):
        for i in self.unit:
            if i.id == id:
                i.level += 1
            return i
        return None


if __name__ == '__main__':

    unit = NetUnit()
    unit.row = 114
    unit.col = 514
    print(json.dumps(unit.__dict__))
