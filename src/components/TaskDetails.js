import { useParams } from 'react-router-dom'
import { useState, useEffect } from 'react'
import Button from './Button'
import EditTask from './EditTask'
import FormatDateString from './FormatDateString'
import FetchTask from './FetchTask'


const TaskDetails = () => {

    //gets the params passed in from the router
    //is a react hook
    let { id } = useParams()
    const [isLoading, setIsLoading] = useState(true);
    const [task, setTask] = useState(null)
    const [showEditTask, setShowEditTask] = useState(false)

    useEffect(() => {
        console.log("using effect")
        // Fetch Task
        const fetchTask = FetchTask

        const getTask = async () => {          
            try {       
            const taskFromServer = await fetchTask(id)         
            setTask(taskFromServer)
            setIsLoading(false)
            } catch (error) {
            console.log("failed") 
            console.log(error);
            }   
        }
      
        getTask()

    }, [id]) 

    return (
        <>
            {!isLoading ? (
                <div>    
                    {!showEditTask && (
                        <div className="taskDetail">
                            <h3>
                                {task.text}
                            </h3>
                            <hr></hr>
                            <p>
                                Location: {task.location}
                            </p>
                            <p>
                                Day: {FormatDateString(task.day)}
                            </p>
                            <p>
                                Details: {task.details !== undefined ? task.details : "None"}
                            </p>    
                            <Button
                                color='green'
                                text='Edit Task'
                                onClick={() => setShowEditTask(!showEditTask)}
                            />
                        </div>
                    )}                               

                    {showEditTask && (
                        <EditTask task={task} onSave={() => {}} onCancel={() => setShowEditTask(!showEditTask)}/>
                    )}        
                </div>) : (
                <div>
                    <h1>Loading ...</h1>
                </div>
            )}           
        </>        
    )
}

export default TaskDetails