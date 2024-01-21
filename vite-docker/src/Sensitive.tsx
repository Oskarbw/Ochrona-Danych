import { Alert, Button, Col, Container, Row } from "react-bootstrap"
import NavigationBar from "./NavigationBar"
import { useContext, useState } from "react"
import { API_BASE_URL } from "./misc"
import axios from "axios"
import { AuthContext } from "./AuthProvider"

function Sensitive() {

    const Context = useContext(AuthContext)

    const [warning, setWarning] = useState("")
    const [info, setInfo] = useState("Are you sure you want to see sensitive data?")
    const [cardNumber, setCardNumber] = useState("XXXXXXXXXXXXXXXX")
    const [documentNumber, setDocumentNumber] = useState("AAAXXXXXX")

    const handleShowSensitive = () => {
        const headers = {
            Authorization: `Bearer ${Context.getUser()!.token}`,
            'Content-Type': 'application/json'
          };

        axios.get(`${API_BASE_URL}/user/sensitive`, { headers })
        .then(response => {
            setInfo("")
            setCardNumber(response.data.cardNumber)
            setDocumentNumber(response.data.documentNumber)
        }, error => {
            setWarning("Error occured")
        })
    }

    const handleHideSensitive = () => {
        setInfo("Are you sure you want to see sensitive data?")
        setCardNumber("XXXXXXXXXXXXXXXX")
        setDocumentNumber("AAAXXXXXX")
    }


    return (
    <>
         <NavigationBar />
        <Container style={{ height: '89.1vh' }}>

            <Col md={{ span: 8, offset: 2 }}>
                <Row>
                { info != "" && 
                        <Alert key="info" variant="info">
                            {info}
                        </Alert>
                    }    

                        
                        <Button onClick={handleShowSensitive} className="btn btn-info">Show sensitive data</Button>
                        <Button onClick={handleHideSensitive} className="btn btn-secondary">Hide sensitive data</Button>
                        <h3>Your card number is: {cardNumber}</h3>
                        <h3>Your document number is: {documentNumber}</h3>
                </Row>

            
            </Col>
            
            <Row className="my-5">
                    { warning != "" && 
                        <Alert key="danger" variant="danger">
                            {warning}
                        </Alert>
                    }      
            </Row>
            
        </Container>
    </>
        )
}

export default Sensitive