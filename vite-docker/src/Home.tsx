import { Alert, Button, Col, Container, Form, Row, Table } from "react-bootstrap"
import NavigationBar from "./NavigationBar"
import { useContext, useEffect, useState } from "react"
import { AuthContext, UserType } from "./AuthProvider"
import { API_BASE_URL } from "./misc"
import axios from "axios"


type TransferType = {
    title : string,
    receiverName : string,
    receiverAdress : string,
    senderName : string,
    receiverAccountNumber : string,
    amount : number,
    time : number
}



function Home() {

    const Context = useContext(AuthContext)

    const [balance, setBalance] = useState("")
    const [transferList, setTransferList] = useState<Array<TransferType>>();
    const [title, setTitle] = useState("")
    const [receiverAccountNumber, setReceiverAccountNumber] = useState("")
    const [receiverName, setReceiverName] = useState("")
    const [receiverAdress, setReceiverAdress] = useState("")
    const [amount, setAmount] = useState(0)

    const [warning, setWarning] = useState("")



    useEffect(() => {
        triggerFetch()
    },[])



    const triggerFetch = () => {
        if (Context.getUser() != null) {

            const headers = {
                Authorization: `Bearer ${Context.getUser()!.token}`,
                'Content-Type': 'application/json'
              };

            axios.get(`${API_BASE_URL}/transfer`, { headers })
                .then(response => {
                    setTransferList(response.data.transfers)
                }, error => {
                    
                });

            
            axios.get(`${API_BASE_URL}/user/balance`, {headers})
            .then(response => {
                setBalance(response.data.balance)
            }, error => {
                    
            });

        }
    }


    const handleAddTransfer = () => {

        const payload = {
            title: title,
            receiverName: receiverName,
            receiverAdress: receiverAdress,
            senderUsername: Context.getUser()?.username,
            receiverAccountNumber: receiverAccountNumber,
            amount: amount
        }
        console.log(payload)

        const headers = {
            Authorization: `Bearer ${Context.getUser()!.token}`,
            'Content-Type': 'application/json'
          };

        axios.post(`${API_BASE_URL}/transfer`, payload, { headers })
            .then(response => {
                
                triggerFetch()
            }, error => {
                
                setWarning(`Error occured`)
            });
    }

    return (
        <>
            <NavigationBar />
            <h1>Balance: {balance} </h1>
            <h3> Account number: {Context.getUser()?.accountNumber}</h3>
            <Container style={{ height: '89.1vh' }}>

                <Col md={{ span: 8, offset: 2 }}>

                    { Context.getUser() != null &&
                    <Row className="my-5">
                        <Col md={{ span: 8, offset: 3 }}>
                            <label htmlFor="input-title">Title</label>
                        <input
                            className="textfield mx-2"
                            type="text"
                            id = "input-title"
                            onChange={(e) => setTitle(e.target.value)}
                        />

                        <label htmlFor="input-receiver-name">Receiver Name</label>
                        <input
                            className="textfield mx-2"
                            type="text"
                            id = "input-receiver-name"
                            onChange={(e) => setReceiverName(e.target.value)}
                        />     

                        <label htmlFor="input-receiver-address">Receiver Address</label>
                        <input
                            className="textfield mx-2"
                            type="text"
                            id = "input-receiver-address"
                            onChange={(e) => setReceiverAdress(e.target.value)}
                        />    

                        <label htmlFor="input-receiver-account-number">Receiver Account Number</label>
                        <input
                            className="textfield mx-2"
                            type="text"
                            id = "input-receiver-account-number"
                            onChange={(e) => setReceiverAccountNumber(e.target.value)}
                        />    

                        <label htmlFor="input-amount">Amount</label>
                        <input
                            className="textfield mx-2"
                            type="number"
                            step="0.01"
                            id = "input-amount"
                            onChange={(e) => setAmount(parseFloat(e.target.value))}
                        />    

                        <Button onClick={() => handleAddTransfer()} className="btn-success" id="todo-edit-btn">
                            Send
                        </Button>
                        </Col>
                    </Row>
                    }
                    

                    <Row>
                    <Table >
                        <thead>
                            <tr>   
                                <th className="col-2">Sender</th>
                                <th className="col-2">Receiver</th>
                                <th className="col-3">Receiver Address</th>
                                <th className="col-2">Receiver Account Number</th>
                                <th className="col-3">Title</th>
                                <th className="col-2">Amount</th>
                                <th className="col-4">Time</th>
                            </tr>
                        </thead>
                        <tbody>
                            { transferList != undefined &&
                            transferList.map((transfer : TransferType, index : number) => (
                                <tr key={index}>
                                    <td>
                                      {transfer.senderName}
                                    </td> 
                                    <td>
                                      {transfer.receiverName}
                                    </td> 
                                    <td>
                                      {transfer.receiverAdress}
                                    </td> 
                                    <td>
                                      {transfer.receiverAccountNumber}
                                    </td> 
                                    <td>
                                      {transfer.title}
                                    </td> 
                                    <td>
                                      {transfer.amount.toString()}
                                    </td> 
                                    <td>
                                        {  new Date(transfer.time * 1000).toISOString().replace(/T/, ' ').replace(/\..+/, '')}
                                    </td> 
                        
                                </tr>
                            ))}
                        </tbody>
                    </Table>
                    </Row>
                    <Row className="my-5">
                    { warning != "" && 
                        <Alert key="danger" variant="danger">
                            {warning}
                        </Alert>
                    }      
                    </Row>
                    
                </Col>
            </Container>
        </>
    )


}

export default Home