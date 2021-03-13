﻿import { useState } from 'react'
import Form from 'react-bootstrap/Form'
import Constant from '../Constant'

const CommentForm = ({ userId, token, taskId }) => {
    const [text, setText] = useState('')

    const onSubmit = async (e) => {
        //e.preventDefault() is so it doesnt actually submit to the page
        e.preventDefault()
        setText('')
        await fetch(Constant() + '/api/comments', {
            method: 'POST',
            headers: {
                'Content-type': 'application/json',
                'Authorization': 'Bearer ' + token
            },
            body: JSON.stringify({ userId, taskId, text })
        })
            .then(data => data.json())
            .catch((error) => {
                console.error('Fetch Error:', error);
            })
    }
    

    return (
        <div className='commentForm'>
            <Form onSubmit={onSubmit}>
                <Form.Group>
                    <Form.Control
                        type='text'
                        maxLength='200'
                        placeholder='Add Reply'
                        value={text}
                        onChange={(e) => setText(e.target.value)} />
                </Form.Group>

                <button
                    type='submit'
                    className='btn btn-block'
                    style={{ backgroundColor: 'skyblue' }}
                >
                    Reply
                </button>
            </Form>
        </div>
    )
}

export default CommentForm