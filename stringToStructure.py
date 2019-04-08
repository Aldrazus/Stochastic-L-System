import wselect
from pathlib import Path


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
    f = open(filename, 'w')
    startFile(f)
    for axiom in range(0, len(lsystem.iterations)):
        for symbol in range(0, len(lsystem[axiom])):
            if materialKey[lsystem[axiom][symbol]] is not "air":
                f.write(xmlify("SquareTiny", materialKey[lsystem[axiom][symbol]],
                               (float(symbol) / len(lsystem[axiom]))*(windowRange[1] - windowRange[0]) + windowRange[0],
                               (float(symbol) / len(lsystem[axiom]))*(windowRange[1] - windowRange[0]) + windowRange[0],
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
