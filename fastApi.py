from fastapi import FastAPI,File,UploadFile
import uvicorn
import numpy as np
from io import BytesIO  
from PIL import Image
from tensorflow import keras
from keras.models import load_model
import requests







class_names=["Early Blight","Late Blight","Healty"]
endpoint="http://localhost:8501/v1/models/potatoes_model:predict"


app = FastAPI()

@app.get("/")
async def read_root():
    return {"message": "I alive"}

@app.post("/predict")
async def predict(
file:UploadFile=File(...)


):
    image= read_file_as_image(await file.read())
    img_batch= np.expand_dims(image,0)  
    json_data={
       "instances":img_batch.tolist()
    }
    response= requests.post(endpoint,json=json_data)
    prediction=np.array(response.json()["predictions"][0])

    predicted_class=class_names[np.argmax(prediction)]
    confidence=np.max(prediction[0])
   



    return {
       "class":predicted_class,
       "confidence":float(confidence)
    }


def read_file_as_image(data) -> np.ndarray:
 image= np.array(Image.open(BytesIO(data)) ) 


 

 return image
if __name__=="__main__":
    uvicorn.run(app,host="localhost",port=8000)