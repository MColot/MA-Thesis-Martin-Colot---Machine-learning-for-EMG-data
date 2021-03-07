# https://simtk.org/projects/hand_muscle
# https://www.ncbi.nlm.nih.gov/pmc/articles/PMC4390324/

import opensim as osim

armModel = osim.Model("OpenSim Hand Model.osim")
armModel.setUseVisualizer(True)

state = armModel.initSystem()



for i in range(armModel.getMuscles().getSize()//2):
    armModel.getMuscles().get(i).setActivation(state, 1)
    armModel.getCoordinateSet().get(i).setValue(state, 40)



armModel.equilibrateMuscles(state)

state.setTime(0)
manager = osim.Manager(armModel)
manager.initialize(state)
state = manager.integrate(10.0)

