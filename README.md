# IoTEdgeStarterKit
Starter kit for iot edge modules

![image](https://user-images.githubusercontent.com/14832274/113845894-96ceaf80-97b3-11eb-8521-9a1fe8407d88.png)

Steps to run the starter kit

Step 1: Provision below resources in Azure
  - IoTHub
  - Ubuntu VM 18.*
  - Azure Function App
  - Storage Account

Step 2: Publish EventProcessor Function Project to the provisioned Function App and update the Application Configuration of the function APP
  - EventHubConnectionString (buitin event hub endpoint of the IoTHub)
  - StorageAccountConntnectionString(Provisioned storage account connection string)
  - IoTHubConnectionString(Provisioned IoTHub connection string)

Step 3: Create messages consumer group for the built-in event hub endpoint and restart the function app

Step 4: Install IoT Edge runtime on Ubuntu machine [link](https://docs.microsoft.com/en-us/azure/iot-edge/how-to-install-iot-edge?view=iotedge-2018-06)

Step 5: Create an IoTEdge device in IoT Hub and update the config.yaml of the IoTEdge runtime [link](https://docs.microsoft.com/en-us/azure/iot-edge/how-to-register-device?view=iotedge-2018-06&tabs=azure-portal).

Step 6: Containerize and publish the thumbnailconverter project to ACR.

Step 7: Open the IoTEdgeStarterKit folder in VSCode update the deployment.template.debug.json with ACR credentials and the image section of thumbnailconvertermodule with the image link of the published image.

Step 8: Add Set IoTHub connection string in the VSCode explorer.

Step 8: Right click on deployment.template.debug.json and select GenerateDeploymentManifest.

Step 9: Right click on the generated deployment manifest under config/deployment.debug.amd64.json and click create deployment for single device and select the device to which it needs to be deployed.

Features
- .NetCore worker as IoT Edge module
- Direct Methods Example in IoT Edge Module
- Desired Properties Callback Example in IoT Edge Module
- IoT Edge Event Processing with the help of a Azure Function
