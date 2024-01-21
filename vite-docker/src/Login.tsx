import { useContext, useState } from "react"
import axios from 'axios'
import { useNavigate } from "react-router-dom"
import { AuthContext, UserType } from "./AuthProvider"

import { API_BASE_URL } from "./misc"
import { Alert, Button, Card, Col, Container } from "react-bootstrap"

type AlertType = string | null;

function Login() {

    const Context = useContext(AuthContext)

    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [isLoading, setIsLoading] = useState(false)
    const [showPasswordSection, setShowPasswordSection] = useState(false)
    const [usernameError, setUsernameError] = useState("")
    const [passwordPatternText, setPasswordPatternText] = useState("")
    const [passwordError, setPasswordError] = useState("")
    const [variant, setVariant] = useState(0)
    const [pattern, setPattern] = useState("")

    const navigate = useNavigate()

    const handleLoginClick = () => {
      
        setPasswordError("")
        if (password == "") {
            setPasswordError("Password should not be empty")
            return;
        }

      const payload = {
        username: username,
        passwordFragment: password,
        variant: variant
      }
      
      setIsLoading(true)
      axios.post(`${API_BASE_URL}/user/login`, payload)
        .then(response => {

          const authenticatedUser: UserType = {
            username: response.data.username,
            token: response.data.token,
            accountNumber: response.data.accountNumber
          };
          Context.login(authenticatedUser);
          navigate("/home");
        }, error => {
           setIsLoading(false)
           setPasswordError(`Error: ${error}`)
        });
    }



    const handleProceed = () => {
        setIsLoading(true)
        axios.get(`${API_BASE_URL}/user/${username}`)
        .then(response => {
            setIsLoading(false)
            setUsernameError("")
            setVariant(response.data.variant)
            setPattern(response.data.pattern)
            setPasswordPatternText(`(Please type in only underscored characters: ${response.data.pattern})`)

            setShowPasswordSection(true)
            
        }, error => {
           setIsLoading(false)
           setShowPasswordSection(false)
           setUsernameError("Username doesn't exist")
        });
    }

    return (
    <>
            <h1>Login</h1>
            
            <div className="form-group">
              <label htmlFor="exampleInput1">Username</label>
              <input onChange={(e)=>setUsername(e.target.value)} type="text" className="form-control" id="exampleInput1" placeholder="Enter username"/>
              <p>Tutaj pojawi sie error: {usernameError}</p>
              <Button onClick={handleProceed} className="btn btn-primary">Proceed</Button>

              { showPasswordSection && 
              <>
              <label htmlFor="exampleInput2">Password {passwordPatternText}</label>
              <input onChange={(e)=>setPassword(e.target.value)} type="password" className="form-control" id="exampleInput2" placeholder="Enter password"/>
              <p>Tutaj pojawi sie error: {passwordError}</p>
              <Button onClick={handleLoginClick} className="btn btn-primary">Login</Button>
              </>
              }
              
            </div>
            { isLoading &&
            <em>LOADING . . .</em>
            }
    </>
      )
}

export default Login