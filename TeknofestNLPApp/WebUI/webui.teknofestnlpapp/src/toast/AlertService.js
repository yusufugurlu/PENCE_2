import Swal from 'sweetalert2';
import { toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

const AlertService = {
  // SweetAlert kullanarak bir uyarı gösterme fonksiyonu
  showAlert(title, text, icon = 'info') {
    Swal.fire({
      title,
      text,
      icon,
      confirmButtonText: 'Tamam'
    });
  },

  // React-Toastify kullanarak bir toast mesajı gösterme fonksiyonu
  showToast(message, type = 'info') {
    toast[type](message);
  }
};

export default AlertService;
