# yosenabe

# 動作確認
yosenabe以下でcatkin_make
echo "source ~/yosenabe/devel/setup.bash" >> ~/.bashrc 

roscore
roslaunch turtlebot_gazebo turtlebot_world.launch 
roslaunch turtlebot_bringup minimal.launch 
を起動した状態で

rosrun tesm_h Environment.py
rosrun team_h Agent.py