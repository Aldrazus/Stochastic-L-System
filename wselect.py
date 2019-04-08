import random

"""
select(tuples): 
    input: sequences of choices and weights, both of same length
    output: selected choice
    Description: Selects from choices with weighted probabilities
    using a lottery system. Note, this is not very precise,
    so use numpy's choice function if precision is necessary.
"""
def select(choices, weights):
    total = 100
    count = 0
    winner = random.randint(0, 99)

    for choice, weight in zip(choices, weights):
        numTickets = weight * total
        if winner < numTickets + count:
            return choice
        else:
            count += numTickets