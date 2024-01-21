import { Alert, Button, Col, Container, Row } from "react-bootstrap"
import NavigationBar from "./NavigationBar"
import { useContext, useState } from "react"
import { API_BASE_URL } from "./misc"
import axios from "axios"
import { AuthContext } from "./AuthProvider"

function ChangePassword() {

    const Context = useContext(AuthContext)

    const [password, setPassword] = useState("")
    const [password2, setPassword2] = useState("")
    const [warning, setWarning] = useState("")
    const [isLoading, setIsLoading] = useState(false)
    const [success, setSuccess] = useState("")


    const handleChangePassword = () => {
        if (password != password2) {
            setWarning("Passwords aren't the same")
            return
        }

        setSuccess("")
        setWarning("")
        setIsLoading(true)
        
        const headers = {
            Authorization: `Bearer ${Context.getUser()!.token}`,
            'Content-Type': 'application/json'
          };

          const payload = {
            password: password
          }

        axios.post(`${API_BASE_URL}/user/changepassword`, payload, { headers })
        .then(response => {
            setSuccess("Successfully changed password")
            setIsLoading(false)
        }, error => {
            setWarning(`Error occured: ${error.response.data}`)
            setIsLoading(false)
        })

    }

    return (
    <>
        <NavigationBar />
        <Container style={{ height: '89.1vh' }}>

            <Col md={{ span: 8, offset: 2 }}>
                <Row>
                
                <label htmlFor="exampleInput1">Password</label>
                    <input onChange={(e)=>setPassword(e.target.value)} type="password" className="form-control" id="exampleInput1" placeholder="Enter new password"/>
                    <input onChange={(e)=>setPassword2(e.target.value)} type="password" className="form-control" id="exampleInput2" placeholder="Retype new password"/>
                    { isLoading ?
                    <Button onClick={handleChangePassword} className="btn btn-warning" disabled>Change password</Button>
                    :
                    <Button onClick={handleChangePassword} className="btn btn-warning" >Change password</Button>
                    }
                </Row>
            </Col>
            
            <Row className="my-5">
                    { warning != "" && 
                        <Alert key="danger" variant="danger">
                            {warning}
                        </Alert>
                    } 

                     { success != "" && 
                        <Alert key="success" variant="success">
                            {success}
                        </Alert>
                    }           
                    </Row>
            
        </Container>
    </>
        )
}

export default ChangePassword