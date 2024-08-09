import logo from './logo.svg';
import './App.css';
import Comments from './companents/comments';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import CustomNavbar from './companents/CustomNavbar';
import CommentsPage from './pages/CommentsPage';

function App() {
  return (
    <Router>
      <CustomNavbar />
      <Routes>
        <Route path="/" exact element={<CommentsPage />} />
        <Route path="/comments" element={<CommentsPage />} />
      </Routes>
    </Router>
  );
}

export default App;
