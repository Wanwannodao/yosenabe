#!/usr/bin/env python
import rospy
import cv2
from sensor_msgs.msg import Image
from cv_bridge import CvBridge, CvBridgeError
from geometry_msgs.msg import Twist
from team_h.srv import *
from std_srvs.srv import Empty

# ====================
# Gazebo Services
# ====================
UNPAUSE='/gezebo/unpause_physics'
PAUSE='/gazebo/pause_physics'
RESET='/gazebo/reset_simulation'


# ====================
# Environment
# ====================
class Environment:

    def __init__(self):
        self.unpause = rospy.ServiceProxy(UNPAUSE, Empty)
        self.pause = rospy.ServiceProxy(PAUSE, Empty)
        self.reset_sim = rospy.ServiceProxy(RESET, Empty)

        self.move_pub = rospy.Publisher("/cmd_vel_mux/input/teleop", Twist, queue_size=2)
        self.env = rospy.Service("step", Env, self.step)
        rospy.Service("reset", Reset, self.reset)
        
        self.bridge = CvBridge()
        
    def step(self, req):
        #print "wait"
        #rospy.wait_for_service(UNPAUSE)
        #print "for service"
        #try:
        #    self.unpause()
        #except rospy.ServiceException, e:
        #    print "%s"%e
        # TODO error handling

        twist = Twist()
        twist.linear.x = req.act
        twist.angular.z = 0

        self.move_pub.publish(twist)

        raw_image = rospy.wait_for_message('/camera/rgb/image_raw', Image, timeout=50)
        #depth_image = rospy.wait_for_message('/camera/depth/image_raw', Image, timeout=50)

        # for test
        cv_image = self.bridge.imgmsg_to_cv2(raw_image, "rgb8")
        cv2.imshow("test", cv_image)
        cv2.imwrite("image.png", cv_image)

        self.state = raw_image
        
        #rospy.wait_for_service(PAUSE)
        #self.pause()

        self.reward = 1
        self.terminal = False

        return EnvResponse(self.state, self.reward, self.terminal)

    def reset(self, req):
        rospy.wait_for_service(RESET)
        self.reset_sim()
        
        #rospy.wait_for_service(UNPAUSE)
        #self.unpause()
        
        self.state = rospy.wait_for_message('/camera/rgb/image_raw', Image, timeout=50)

        #rospy.wait_for_service(PAUSE)
        #self.pause()

        return ResetResponse(self.state)

if __name__ == "__main__":
    rospy.init_node("env")
    env = Environment()
    rospy.spin()
        
        
