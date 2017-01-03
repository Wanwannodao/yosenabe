#!/usr/bin/env python
import rospy
from team_h.srv import *

# ====================
# Agent
# ====================
class Agent:
    #def __init__(self):
    def predict(self, states):
        return 10

# ====================
# 
# ====================
def reset():
    rospy.wait_for_service("reset")
    try:
        reset = rospy.ServiceProxy("reset", Reset)
        res = reset()
        state = res.state
    except rospy.ServiceException, e:
        print "failed %e"%e

    print "env reset"
    return state

def step(act):
    rospy.wait_for_service("step")
    try:
        step = rospy.ServiceProxy("step", Env)
        res = step(act)
        state = res.state
        reward = res.reward
        terminal = res.terminal
    except rospy.ServiceException, e:
        print "failed %e"%e
        
    # for debug
    print "state fetched"
    return state, reward, terminal
        
# ====================
# main loop
# ====================
EPISODE=1
STEP=5

def run(agent):
    # (s, a, r, s_) -> replay mem.
    s = reset()
    R = 0

    for j in range(STEP):
        
        a = agent.predict(s)
        s_, r, terminal = step(a)
        
        # (s, a, r, s_) -> replay mem.
        # update
        
        s = s_
        R += r
        
        if terminal:
            break

    print("Total Reward:", R)

if __name__ == "__main__":
    agent = Agent()

    for i in range(EPISODE):
        run(agent)
    
    
    

