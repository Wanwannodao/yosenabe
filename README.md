# yosenabe


### 1\. Install requirements

```
pip install -r lis2/python-agent/requirements.txt
```

### 2\. Download pre-trained model

```
cd lis2/gym_client/examples/notebook
if [ ! -d data ]; then mkdir data; fi; cd data
wget https://dl.dropboxusercontent.com/u/2498135/faster-rcnn/VGG16_faster_rcnn_final.model
cd ..
```

### 2\. Open Unity scene and run

Open unity-sample-environment with Unity and load Scenes/sample_chair.


### 3\. Run Lis_dqn_ImageInput.ipynb using jupyter notebook

lis2/gym_client/examples/notebook/Lis_dqn_ImageInput.ipynb

