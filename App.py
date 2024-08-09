from flask import Flask, request, jsonify
import pandas as pd
import json
import torch
from transformers import BertTokenizer, BertForSequenceClassification
import logging
import gc
import psutil
import os

app = Flask(__name__)
logging.basicConfig(filename='comments.log', level=logging.ERROR)


# Modelin bulunduğu dosya yolu
MODEL_PATH = r"E:\Trendyol_Quality_Paper\egitilmis_model3.pth"



def write_to_file(data):
    with open("comments.txt", 'a', encoding='utf-8') as file:
        file.write(data)
        file.write("\n")
        
def write_to_file_Memory_Usage(data):
    with open("Memory_Usage.txt", 'a', encoding='utf-8') as file:
        file.write(str(data))
        file.write("\n")
        
def bytes_to_gb(bytes):
    return bytes / (1024**3)

# Bellek izleme fonksiyonu
def monitor_memory():
    process = psutil.Process(os.getpid())
    data= "Memory Usage:", process.memory_info().rss / (1024 ** 2), "MB"
    #write_to_file_Memory_Usage(data)

# Gereksiz nesneleri temizleme
def clear_memory():
    gc.collect()  # Garbage collector'ı çalıştır
    torch.cuda.empty_cache()  # CUDA belleğini temizle (varsa)
    
# Modeli ve tokenizer'ı yükleme işlemi
def load_model():
    try:
        # Modeli yükle
        model = BertForSequenceClassification.from_pretrained('dbmdz/bert-base-turkish-128k-uncased')
        model.load_state_dict(torch.load(MODEL_PATH))
        
        # GPU varsa modeli CUDA cihazına taşı
        device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
        model.to(device)
        
        # Tokenizer'ı yükle
        tokenizer = BertTokenizer.from_pretrained('dbmdz/bert-base-turkish-128k-uncased')
        
        # Bellek kullanımını izleme
        memory_usage = torch.cuda.memory_allocated() if torch.cuda.is_available() else 0
        memory_usage_gb = bytes_to_gb(memory_usage)
        logging.info(f"Bellek kullanımı: {memory_usage_gb} GB")

        write_to_file(f"Bellek kullanımı: {memory_usage_gb} GB")
        write_to_file("Model yükleme işleme başarılı.")    
        
        return model, tokenizer,device
    except Exception as e:
        print("Model yükleme işlemi başarısız oldu:", e)
        with open('comments.txt', 'a', encoding='utf-8') as file:
            file.write("Tahmin etme tamamlanamadı. Hata: {}\n".format(str(e)))   
        return None, None,None


# Flask uygulamasının başlangıcında modeli yükle
model, tokenizer,device = load_model()
    
@app.route('/')
def index():
    return 'Flask API çalışıyor!'


# Örnek bir hata işleme fonksiyonu
@app.errorhandler(Exception)
def handle_error(e):
    error_message = str(e)
    return jsonify(handle_error(error_message)), 500


# HTTP 500 hatalarını ele alma fonksiyonu
@app.errorhandler(500)
def handle_500_error(e):
    error_message = str(e)
    return jsonify(handle_error(error_message)), 500


# Genel hata işleme fonksiyonu
def handle_error(error_message):
    # İstek metodunu alın
    request_method = request.method
    
    # İstek URL'ini alın
    request_url = request.url
    
    # Hata mesajını ve diğer bilgileri bir log dosyasına kaydedin
    with open('error.log', 'a', encoding='utf-8') as file:
        file.write(f"{request_method} request to {request_url} failed with error: {error_message}\n")
    
    # Hata mesajını DTO'ya dönüştürün
    error_dto = {'message': 'Bir hata oluştu. Lütfen daha sonra tekrar deneyin.', 'error': error_message}
    
    return error_dto



@app.route('/api/comments', methods=['POST'])
def receive_comments():
 
    try:
        # JSON verisini al
        received_comments = request.json
        
        write_to_file("----------")    
        write_to_file("İşlem başladı")
            
        # Eğer gelen veri boş ise hata döndür
        if not received_comments:
            return handle_error("Boş veri gönderilemez")
        
        
        write_to_file("Veri alındı.")
        # Gelen JSON verisini DataFrame'e dönüştür
        new_df = pd.read_json(json.dumps(received_comments), encoding='utf-8')
               
        write_to_file("Yorumlar tahmin etme işlemi başladı.")

        monitor_memory()        
                             
        # Yorumları tahmin et
        predicted_labels = predict_labels(new_df['Comment'])
        clear_memory()
            
        write_to_file("Tahmin etme işlemi bitti.")
            
        # Tahminleri yeni DataFrame'e ekle
        new_df['Label'] = predicted_labels
        
        write_to_file("İşlem tamamlandı. ")
        
        write_to_file("--------")
        
        # Başarılı yanıt döndür
        return jsonify({'message': 'Yorumlar başarıyla eklendi','error': '' ,'comments': new_df.to_dict(orient='records')}), 200
    except Exception as e:
        logging.error("Bir hata oluştu: %s", e)
        return handle_error('Bir hata oluştu: {}'.format(str(e)))



def predict_labels(comments):
    try:
        predictions = []
        for comment in comments:
            inputs = tokenizer(comment, return_tensors="pt", padding=True, truncation=True)
            inputs = {key: tensor.to(device) for key, tensor in inputs.items()}                         
            with torch.no_grad():
                outputs = model(**inputs)
            prediction = torch.argmax(outputs.logits, dim=1)
            predictions.append(prediction.item())
        return predictions
    except Exception as e:
        logging.error("Tahminleme sırasında bir hata oluştu: %s", e)
        print("Tahminleme sırasında bir hata oluştu:", e)
        with open('comments.txt', 'a', encoding='utf-8') as file:
            file.write("Tahmin etme tamamlanamadı. Hata: {}\n".format(str(e)))
        return []


if __name__ == '__main__':
    app.run(debug=True)