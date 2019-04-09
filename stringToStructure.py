import wselect
import random
import numpy
from pathlib import Path

# blocks number and size
blocks = {'1':[0.84,0.84], '2':[0.85,0.43], '3':[0.43,0.85], '4':[0.43,0.43],
          '5':[0.22,0.22], '6':[0.43,0.22], '7':[0.22,0.43], '8':[0.85,0.22],
          '9':[0.22,0.85], '10':[1.68,0.22], '11':[0.22,1.68],
          '12':[2.06,0.22], '13':[0.22,2.06]}

# blocks number and name
# (blocks 3, 7, 9, 11 and 13) are their respective block names rotated 90 derees clockwise
block_names = {'1':"SquareHole", '2':"RectFat", '3':"RectFat", '4':"SquareSmall",
               '5':"SquareTiny", '6':"RectTiny", '7':"RectTiny", '8':"RectSmall",
               '9':"RectSmall",'10':"RectMedium",'11':"RectMedium",
               '12':"RectBig",'13':"RectBig"}

# defines the levels area (ie. space within which structures/platforms can be placed)
level_width_min = -3.0
level_width_max = 9.0
level_height_min = -3.5         # only used by platforms, ground structures use absolute_ground to determine their lowest point
level_height_max = 6.0

class LSystem:

    def __init__(self, axiom, rules):
        self.rules = rules
        self.iterations = [axiom]

    def iterate(self):
        newAxiom = ''
        for symbol in self.iterations[-1]:
            if symbol in rules:
                newAxiom = ''.join([newAxiom, wselect.select(self.rules[symbol][0], self.rules[symbol][1])])
            else:
                newAxiom = ''.join([newAxiom, symbol])
        self.iterations.append(newAxiom)

    def __getitem__(self, index):
        return self.iterations[index]


"""
searches and replaces symbols w, s, i, a with wood, stone, ice, air respectively
"""
def stringToStructure(lsystem, filename):
    materialKey = {'W': "wood", 'S':"stone", 'I':"ice", 'A':"air"}
    windowRange = [-4, 4]
    xPos = random.uniform(-4, 4)
    yPos = -3.5 + len(lsystem.iterations) * 0.22
    f = open(filename, 'w')
    startFile(f)
    for axiom in range(0, len(lsystem.iterations)):
        yPos = yPos - axiom * 0.22
        for symbol in range(0, len(lsystem[axiom])):
            if materialKey[lsystem[axiom][symbol]] is not "air":
                f.write(xmlify("SquareTiny", materialKey[lsystem[axiom][symbol]],
                               xPos - ((len(lsystem[axiom]) / 2) - symbol) * 0.22,
                               yPos,
                               0))
    endFile(f)
    f.close()


def xmlify(blockType, material, x, y, rot):
    return '<Block type="{}" material="{}" x="{}" y="{}" rotation="{}" />\n'.format(blockType, material, str(x), str(y), str(rot))


def startFile(file):
    file.write('<?xml version="1.0" encoding="utf-16"?>\n')
    file.write('<Level width ="2">\n')
    file.write('<Camera x="0" y="2" minWidth="20" maxWidth="30">\n')
    file.write('<Birds>\n')
    file.write('<Bird type="BirdRed"/>\n')
    file.write('<Bird type="BirdRed"/>\n')
    file.write('<Bird type="BirdRed"/>\n')
    file.write('</Birds>\n')
    file.write('<Slingshot x="-8" y="-2.5">\n')
    file.write('<GameObjects>\n')

def endFile(file):
    file.write('</GameObjects>\n')
    file.write('</Level>\n')


rules = {'S': [['S', 'ISI'], [.80, .20]], 'W': [['W', 'SWS'], [.60, .40]], 'I': [['I', 'IAIAI'], [.20, .80]]}
lsystem = LSystem('W', rules)

for i in range(0, 5):
    lsystem.iterate()

for axiom in lsystem.iterations:
    print(axiom + '\n')

stringToStructure(lsystem, Path("./levels/level-04.xml"))
