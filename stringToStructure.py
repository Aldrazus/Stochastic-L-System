import wselect


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


rules = {'A': [['BB', 'BAB'], [.80, .20]], 'B': [['AA', 'ACA'], [.60, .40]], 'C': [['ABA', 'BCB'], [.50, .50]]}
lsystem = LSystem('A', rules)

for i in range(0, 5):
    lsystem.iterate()

for axiom in lsystem.iterations:
    print(axiom + '\n')