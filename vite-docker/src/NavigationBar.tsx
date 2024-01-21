import { useContext } from 'react';
import { Col } from 'react-bootstrap';
import Container from 'react-bootstrap/Container';
import Nav from 'react-bootstrap/Nav';
import Navbar from 'react-bootstrap/Navbar';
import NavDropdown from 'react-bootstrap/NavDropdown';
import { AuthContext } from './AuthProvider';
import { useNavigate } from 'react-router-dom';


function NavigationBar() {

    const Context = useContext(AuthContext)
    const navigate = useNavigate()

    const handleLogout = () => {
        Context.logout()
        navigate("/")
    }

    const handleHomePage = () => {
        navigate("/home")
    }

    const handleSensitive = () => {
        navigate("/sensitive")
    }

    const handleChangePassword = () => {
        navigate("/changepassword")
    }
    

    return (
         <Navbar expand="lg" className="bg-body-tertiary">
            <Container>
                <Col xs={8}>
                    <Navbar.Brand onClick={(handleHomePage)}><strong>BANKER</strong></Navbar.Brand>
                    <i className="bi bi-bank"></i>
                </Col>
                
                <Col xs={1}>
                    <Nav className="me-auto">
                        <NavDropdown title={Context.getUser()?.username ?? "You're not logged in"} id="basic-nav-dropdown">
                            <NavDropdown.Item onClick={handleSensitive}>See sensitive data</NavDropdown.Item>
                            <NavDropdown.Item onClick={handleChangePassword}>Change password</NavDropdown.Item>
                            <NavDropdown.Divider/>
                            <NavDropdown.Item onClick={handleLogout}>Logout</NavDropdown.Item>
                            
                        </NavDropdown>
                    </Nav>
                </Col>
            </Container>
        </Navbar>
    )
}

export default NavigationBar