let roomCode
let userId

const connection = new signalR.HubConnectionBuilder().withUrl("/voting-hub").build()
connection.on("StateUpdate", (data) => {
    console.log(data)
})

const init = (newRoomCode, newUserId) => {
    roomCode = newRoomCode
    userId = newUserId
    console.log(roomCode, userId)
    connection.start().then(() => {
        console.log('connected')
        connection.invoke("ConnectToRoom", roomCode)
    })
}
