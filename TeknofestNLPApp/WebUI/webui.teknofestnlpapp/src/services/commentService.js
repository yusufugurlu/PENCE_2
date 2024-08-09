import axios from 'axios';

const commentService = {
    // URL'ye GET isteği gönderen fonksiyon
    async getData(data) {
        try {
            let url = 'http://localhost:5001/api/Comment/GetProductInComments';
            const response = await axios.post(url, data);
            // Veriyi döndür
            return response.data;
        } catch (error) {
            // Hata durumunda hata nesnesini döndür
          return {};
        }
    },
    
    async getProductInCommentsWithPagination(data) {
        try {
            let url = 'http://localhost:5001/api/Comment/GetProductInCommentsWithPagination';
            const response = await axios.post(url, data);
            // Veriyi döndür
            return response.data;
        } catch (error) {
            // Hata durumunda hata nesnesini döndür
          return {};
        }
    }
};

export default commentService;