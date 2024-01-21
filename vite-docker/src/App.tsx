import { Route, RouterProvider, createBrowserRouter, createRoutesFromElements } from 'react-router-dom'
import Home from './Home';
import AuthProvider from './AuthProvider';
import Login from './Login';
import Sensitive from './Sensitive';
import ChangePassword from './ChangePassword';

function App() {



  const router = createBrowserRouter(
    createRoutesFromElements(
      <>
        <Route path="/" element={<Login />} />
        <Route path="/login" element={<Login />} />
        <Route path="/home" element={<Home />} />
        <Route path="/sensitive" element={<Sensitive />} />
        <Route path="/changepassword" element={<ChangePassword />} />
      </>
  ));
  

  return (
    <AuthProvider>
      <RouterProvider router={router} />
    </AuthProvider>
  )
}

export default App
