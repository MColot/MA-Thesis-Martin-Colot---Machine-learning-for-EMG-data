# https://simtk.org/projects/hand_muscle
# https://www.ncbi.nlm.nih.gov/pmc/articles/PMC4390324/

import opensim as osim

armModel = osim.Model("OpenSim Hand Model.osim")
armModel.setUseVisualizer(True)

state = armModel.initSystem()

manager = osim.Manager(armModel)
manager.initialize(state)
state = manager.integrate(10.0)
