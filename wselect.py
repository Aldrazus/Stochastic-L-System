import random

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