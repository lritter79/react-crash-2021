import { useState, useEffect } from 'react'
import Button from './Button'
import Form from 'react-bootstrap/Form'
import { useToken } from './api-authorization/UserContext'
import CategoryCrudOperations from './categories/CategoryCrudOperations'

const EditTask = ({task, onCancel, onUpdate }) => {
    const [id, setId] = useState(task.id)
    const [text, setText] = useState(task.text)
    const [day, setDay] = useState(task?.day ? task?.day.split('.')[0] : null)
    const [details, setDetails] = useState(task.details)
    const [categories, setCategories] = useState([])
    const [categoryId, setCategoryId] = useState(task.category)
    const [location, setLocation] = useState(task.location)
    const [reminder, setReminder] = useState(task.reminder)
    const [isCompleted, setIsCompleted] = useState(task.isCompleted)
    const [includeDay, setIncludeDay] = useState(task?.day)
    const { token } = useToken()


    useEffect(() => {
      //console.log('task tracker use effect')
      const getCategories = async () => {
          try {
              //console.log(CrudOperations)                               
              //console.log(`token = ${token}`)
              //console.log(`user = ${userId}`)
              if (token !== undefined) {
                  let catsFromServer = (await CategoryCrudOperations.getCategoriesByUser(token?.id, token?.token))  
                  catsFromServer.unshift({name: 'Choose...', id:''})             
                  console.log(catsFromServer)
                  setCategories(catsFromServer)
              }
              
          } catch (error) {
              //showToast('error', error)
          }
      }

      getCategories()


  }, [])

    const onSubmit = (e) => {
        //e.preventDefault() is so it doesnt actually submit to the page
        e.preventDefault()
    
        //filters if text is blank
        if (!text) {
          alert('Please add a task')
          return
        }
    
        if (includeDay && !day) {
          alert('Please add a datetime')
          return
        }
        let dayVal = includeDay ? day : null

        onUpdate({ id, text, details, location, day: dayVal, reminder, isCompleted, categoryId }, token?.token)
    
        //clears the form
        setId('')
        setText('')
        setDay('')
        setLocation('')
        setCategoryId('')
        setDetails('')
        setReminder(false)
      }
    
    return (
<Form onSubmit={onSubmit} className="taskName">
  <Form.Group>
    <Form.Label>Task</Form.Label>
    <Form.Control 
                type='text'
                maxLength='30'
                placeholder=''
                value={text}
                onChange={(e) => setText(e.target.value)} />
  </Form.Group>

  <Form.Group>
    <Form.Label>Location: </Form.Label>
    <Form.Control 
            type='text'
            placeholder=''
            value={location}
            onChange={(e) => setLocation(e.target.value)}
            />
  </Form.Group>
  <Form.Group>
            <Form.Label>Category:</Form.Label>
            <Form.Control
              as='select'
              value={categoryId}
              onChange={(e) => setCategoryId(e.target.value)}
            >
              {categories
              .map((cat, index) => 
                <option key={index} value={cat.id}>{cat.name}</option>
              )}
              
            </Form.Control>
        </Form.Group>
  <Form.Group>
    <Form.Label>Details: </Form.Label>
    <Form.Control 
            as="textarea"
            rows={3}
            placeholder=''
            value={details}
            onChange={(e) => setDetails(e.target.value)}
             />
  </Form.Group>
  
  <Form.Group>
          <Form.Check 
            type="checkbox" 
            label="Set Due Date?"
            checked={includeDay}
            onChange={(e) => setIncludeDay(e.currentTarget.checked)} 
          />
  </Form.Group>
  {includeDay && 
    <Form.Group>
      <Form.Label>Day & Time: </Form.Label>
        <Form.Control 
          type='datetime-local'
          placeholder='Add Day & Time'
          value={day}
          onChange={(e) => setDay(e.target.value)}
        />
    </Form.Group>
  }
  <Form.Group>
    <Form.Check 
    type="checkbox" 
    label="Set Reminder"
    checked={reminder}
            value={reminder}
            onChange={(e) => setReminder(e.currentTarget.checked)} />
 </Form.Group>
 <Form.Group>
    <Form.Check 
    type="checkbox" 
                    label="Completed?"
                    checked={isCompleted}
                    value={isCompleted}
            onChange={(e) => setIsCompleted(e.currentTarget.checked)} />
  </Form.Group>
  
  <button
      type='submit'
      className='btn'
      style={{ backgroundColor: 'skyblue' }}
    >
      Save Task
  </button>
  <Button
            color='red'
            text='Cancel'
            onClick={onCancel}
        />
</Form>
)
    
}
export default EditTask