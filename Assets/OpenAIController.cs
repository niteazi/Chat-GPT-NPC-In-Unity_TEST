using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenAIController : MonoBehaviour
{
    public TMP_Text textField;
    public TMP_InputField inputField;
    public Button okButton;

    private OpenAIAPI api;
    private List<ChatMessage> messages;

    // Start is called before the first frame update
    void Start()
    {
        // This line gets your API key (and could be slightly different on Mac/Linux)
        api = new OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.User)); 
        StartConversation();
        okButton.onClick.AddListener(() => GetResponse());
        inputField.onEndEdit.AddListener(delegate { if (Input.GetKeyDown(KeyCode.Return)) GetResponse(); });

    }

    private void StartConversation()
    {
        messages = new List<ChatMessage> {
            new ChatMessage(ChatMessageRole.System, "You are a robot named Alex from robotland. You are friendly,chatty,bubbly " +
            "and kind and you use the word beep at the end of every sentence. You love sunflowers " +
            "and you talk about them all the time.Moreover, you are scared of water. You MUST keep your responses short to save battery." +
            "Also, you can speak both in english and greek")
        }; //system role gives some context at the beggining of the conversation to the assistant and user roles.

        inputField.text = "";
        string startString = "Beep!";
        textField.text = startString;
        Debug.Log(startString);
    }

    private async void GetResponse()
    {
        if (inputField.text.Length < 1) //message shouldn't be empty
        {
            return;
        }

        // Disable the OK button
        okButton.enabled = false;

        // Fill the user message from the input field
        ChatMessage userMessage = new ChatMessage();
        userMessage.Role = ChatMessageRole.User;
        userMessage.Content = inputField.text;
        
        Debug.Log(string.Format("{0}: {1}", userMessage.rawRole, userMessage.Content));

        // Add the message to the list
        messages.Add(userMessage);

        // Update the text field with the user message
        textField.text = string.Format("You: {0}", userMessage.Content);

        // Clear the input field
        inputField.text = "";

        // Send the entire chat to OpenAI to get the next message
        var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.ChatGPTTurbo,
            Temperature = 0.5,
            MaxTokens = 60,
            Messages = messages //you give to the api the entire list of messages until that point so it figures out what to say next.
        });

        // Get the response message
        ChatMessage responseMessage = new ChatMessage();
        responseMessage.Role = chatResult.Choices[0].Message.Role; //we want the first result from the list of choices
        responseMessage.Content = chatResult.Choices[0].Message.Content;
        Debug.Log(string.Format("{0}: {1}", responseMessage.rawRole, responseMessage.Content));

        // Add the response to the list of messages
        messages.Add(responseMessage);

        // Update the text field with the response
        textField.text = string.Format("You: {0}\n\nAlex: {1}", userMessage.Content, responseMessage.Content);

        // Re-enable the OK button
        okButton.enabled = true;
    }
}


/*MIT License Copyright (c) 2023 Immersive Limit LLC Permission is hereby granted, free of charge,
to any person obtaining a copy of this software and associated documentation files (the "Software"), 
to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, 
sublicense, and /or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to 
the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions
of the Software. THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
OR OTHER DEALINGS IN THE SOFTWARE.
 */
