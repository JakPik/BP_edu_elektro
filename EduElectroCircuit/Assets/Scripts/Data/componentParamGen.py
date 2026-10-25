import random

R = 15
resistors = [
  #  20000, 10000,
   # 5000,2500,2000,1000,
    500,396,246,175,
    55,50,25,
    5,2
]


def SerialResistance(seriesLength):
    sum = 0
    serial = []
    for _ in range(seriesLength):
        item = resistors[random.randint(0,len(resistors) - 1)]
        serial.append(item)
        sum+= item
    return (sum, ("Serial resistors", serial))

def ParalelResistance(branches):
    sum = 0
    paralel = []
    branchSum = []
    b = 1
    for branch in branches:
        obj = []
        item = 0
        sum = 0
        for _ in range(branch):
            item = resistors[random.randint(0,len(resistors) - 1)]
            obj.append(item)
            sum+= item
        branchSum.append(sum)
        paralel.append(("branch: " + str(b), obj ))
        b+= 1
    sum = 0
    for i in branchSum:
        sum += pow(i,-1)
    sum = pow(sum, -1)
    return (sum, ("Paralel resistors", paralel))

def calculateSeriesR(U, Ur, R):
    return (Ur*R)/(U-Ur)

def calculateParalR(Rx, branches, U, Ur):
    B = []
    obj = 0
    for t in branches:
        item = 0
        for _ in range(0, t):
            obj = resistors[random.randint(0,len(resistors) - 1)]
            usedItems.append(obj)
            item += obj
        B.append(item)
        
def genRxInSeries(numberOfGens, U, Ur, serialR, branches):
    print(f"Gen List: U: {U}, Ur: {Ur}")
    for i in range(numberOfGens):
        usedItems = []
        R_total = 0
        Rx = 0
        variable = SerialResistance(serialR)
        R_total = variable[0]
        usedItems.append(variable[1])
        for branching in branches:
            variable = ParalelResistance(branching)
            R_total += variable[0]
            usedItems.append(variable[1])
        Rx = calculateSeriesR(U,Ur,R_total)
        I = U/((Rx)+R_total)
        if round(Rx, 2) == Rx:
            print(f"{i + 1}: R_total: {R_total}, Rx: {Rx}, I: {I} \nComponents: {usedItems}\n")

genRxInSeries(100, 12, 4, 3, [(2, 1)])