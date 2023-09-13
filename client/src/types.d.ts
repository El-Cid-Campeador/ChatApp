type UserData = {
    id: string,
    username: string,
    email: string,
    firstName: string,
    lastName: string
    profilePicPath?: string
}

type OnlineUser = {
    id: string,
    username: string,
    firstName: string,
    lastName: string
    profilePicPath?: string
}

type Message = {
    id: string,
    senderId: string,
    content: string,
    date: Date
}
