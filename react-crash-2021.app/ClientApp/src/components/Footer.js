import { Link } from 'react-router-dom'


const Footer = ({ isLoggedIn }) => {
  return (
    <footer>
<<<<<<< HEAD
          <Link to="/" exact>Home</Link> | <Link to="/about" exact>About</Link>
          {isLoggedIn ? (<> | <Link to="/userManager" exact>Manage Account</Link> | <Link to="/alerts" exact>Alerts</Link></>) : (
                  <> | <Link to = "/login" exact>Login</Link> | <Link to="/register" exact>Register</Link></>                           
=======
          <Link to="/" >Home</Link> | <Link  to="/about" >About</Link>
          {isLoggedIn ? (<></>): (
              <> | <Link  to="/login" >Login</Link> | <Link  to="/register" >Register</Link></>                           
>>>>>>> 8d1a87fed4171d1a9d12d2b4123d310c2606f5ae
          )}
      <br></br>
      <p>Made with <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-suit-heart-fill" viewBox="0 0 16 16">
              <path d="M4 1c2.21 0 4 1.755 4 3.92C8 2.755 9.79 1 12 1s4 1.755 4 3.92c0 3.263-3.234 4.414-7.608 9.608a.513.513 0 0 1-.784 0C3.234 9.334 0 8.183 0 4.92 0 2.755 1.79 1 4 1z" />
          </svg> By <a href="https://github.com/lritter79">Levon Ritter</a>
      </p>
    </footer>
  )
}

export default Footer
