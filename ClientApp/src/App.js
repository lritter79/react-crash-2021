/// <reference path="components/navbarlinks.js" />

import { useState, useEffect } from 'react'
import { BrowserRouter as Router, Route, NavLink, Redirect } from 'react-router-dom'
import { Navbar, Nav } from 'react-bootstrap'
//for authorization
//import authService from './api-authorization/AuthorizeService'
import Header from './components/Header'
import Footer from './components/Footer'
import Tasks from './components/Tasks'
import Constant from './components/Constant'
import AddTask from './components/AddTask'
import TaskDetails from './components/TaskDetails'
import About from './components/About'
import FetchTask from './components/FetchTask'
import useToken from './components/api-authorization/UseToken'
import RegisterAndLoginRoutes from './components/RegisterAndLoginRoutes'

//import UpdateTask from './components/UpdateTask'
//function setToken(userToken) {
//    sessionStorage.setItem('token', JSON.stringify(userToken));

//}

//function getToken() {

//    const tokenString = sessionStorage.getItem('token');
//    const userToken = JSON.parse(tokenString);
//    //.? is the optional chain operator: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Operators/Optional_chaining
//    /*
//     You need to use the optional chaining operator�?.�when accessing the token property because when you 
//     first access the application, the value of sessionStorage.getItem('token') will 
//     be undefined. If you try to access a property, you will generate an error.
//     */
//    return userToken?.token
//}
//import header and use it like an xml tag
//keeps tasks at the highest level (state)
//changes the state of tasks
//calls fetch tasks whihc returns a promise
//sets tasks as the state
const App = () => {
    const [showAddTask, setShowAddTask] = useState(false)
    const [tasks, setTasks] = useState([])
    const [isLoading, setIsLoading] = useState(true);
    //this is for authentication, see: https://www.digitalocean.com/community/tutorials/how-to-add-login-authentication-to-react-applications
    const { token, setToken } = useToken();
    

    useEffect(() => {
        console.log('using effect in app');

        const getTasks = async () => {
            try {
                const tasksFromServer = await fetchTasks()
                setIsLoading(false)
                setTasks(tasksFromServer)
            } catch (error) {
                console.log("failed")
                console.log(error)
            }
        }
        getTasks()

    }, [])

    // Fetch Tasks
    //gets the tasks we have on the server with async java
    const fetchTasks = async () => {
        const res = await fetch(Constant() + '/api/tasks')
        const data = await res.json()

        return data
    }

    // Fetch Task
    const fetchTask = FetchTask

    // Add Task
    //post because we're adding tasks
    //turns it from js object into json string
    const addTask = async (task) => {
        const res = await fetch(Constant() + '/api/tasks', {
            method: 'POST',
            headers: {
                'Content-type': 'application/json',
            },
            body: JSON.stringify(task),
        })

        //data returned is the new task
        const data = await res.json()
        //take existings takes and add data on
        setTasks([...tasks, data])
        setShowAddTask(false)
        // const id = Math.floor(Math.random() * 10000) + 1
        // const newTask = { id, ...task }
        // setTasks([...tasks, newTask])
    }

    // Delete Task
    //takes in an id
    const deleteTask = async (id) => {
        await fetch(`${Constant()}/api/tasks/${id}`, {
            method: 'DELETE',
        })
        //.filter removes the task with the same id as the id passed up
        setTasks(tasks.filter((task) => task.id !== id))
    }

    // Toggle Reminder
    //takes id so it knows which on to toggle
    const toggleReminder = async (id) => {
        const taskToToggle = await fetchTask(id)
        const updTask = { ...taskToToggle, reminder: !taskToToggle.reminder }
        //const updateTask = UpdateTask
        //update is put
        // 
        const res = await fetch(`${Constant()}/api/tasks/${id}`, {
            method: 'PUT',
            headers: {
                'Content-type': 'application/json',
            },
            body: JSON.stringify(updTask),
        })

        const data = await res.json()

        setTasks(
            tasks.map((task) =>
                task.id === id ? { ...task, reminder: data.reminder } : task
            )
        )
    }

    const updateTask = async (task) => {
        console.log(task)
        setTasks(
            tasks.map((oldTask) => task.id === oldTask.id ? task : oldTask)
        )
    }

    //if there are no tasks, it shows  'No Tasks To Show'
    //short ternary in jsx:
    // {x === y ? (<Thing />) : ('String')}
    //wrap everything in <Router> to use routes
    //exact menas match path exactly
    return (

        <Router>
            <Navbar bg="light" expand="lg">
                <Navbar.Brand>Task Tracker</Navbar.Brand>
                <Navbar.Toggle aria-controls="basic-navbar-nav" />
                <Navbar.Collapse id="basic-navbar-nav">
                    <Nav className="mr-auto">        
                        {token ? (
                            <>
                                <Nav.Link as={NavLink} to="/logout" exact>Logout</Nav.Link>
                            </>) : (<>
                                <Nav.Link as={NavLink} to="/login" exact>Login</Nav.Link>
                                <Nav.Link as={NavLink} to="/register" exact>Register</Nav.Link>
                            </>)}
                        <Nav.Link as={NavLink} to="/" exact>Home</Nav.Link>
                        <Nav.Link as={NavLink} to="/about" exact>About</Nav.Link>                                               
                    </Nav>
                </Navbar.Collapse>
            </Navbar>
            <div className='container'>
                <Route path='/about' exact component={About} />
                {token ? (
                    <>
                        <Route path='/' exact
                            render={(props) => (
                                <>
                                    <Header
                                        onAdd={() => setShowAddTask(!showAddTask)}
                                        showAdd={showAddTask} />
                                    <AddTask onAdd={addTask} isToggled={showAddTask} />
                                    {!isLoading ? (
                                        (tasks.length > 0) ? (
                                            <Tasks
                                                tasks={tasks}
                                                onDelete={deleteTask}
                                                onToggle={toggleReminder}
                                                onGoToDetail={() => { setShowAddTask(false) }} />) :
                                            ('No Tasks To Show')
                                    ) : ('Loading ...')}
                                </>
                                
                            )} />
                        
                        <Route path='/task/:id' exact
                            render={(props) => (
                                <TaskDetails
                                    onUpdate={updateTask} />
                            )} />
                        <Footer />
                    </>
                ) : (
                        <RegisterAndLoginRoutes setToken={setToken} />
                    )}
            </div>
        </Router>
    )
}

export default App

