import socket
import select

HOST = '127.0.0.1'
PORT = 5555


def receive_message(client_socket):
    try:
        message = client_socket.recv(1024).decode('utf-8')
        return message
    except:
        return False
    


server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
server_socket.bind((HOST, PORT))
server_socket.listen()

sockets_list = [server_socket]
clients = {}

print(f"שרת צ'אט מאזין על {HOST}:{PORT}")

while True:
    read_sockets, _, exception_sockets = select.select(sockets_list, [], sockets_list)

    for notified_socket in read_sockets:
        if notified_socket == server_socket:
            client_socket, client_address = server_socket.accept()
            sockets_list.append(client_socket)
            clients[client_socket] = client_address
            print(f"חיבור חדש מ: {client_address}")
        else:
            message = receive_message(notified_socket)
            if message is False:
                print(f"לקוח {clients[notified_socket]} התנתק")
                sockets_list.remove(notified_socket)
                del clients[notified_socket]
                continue

            print(f"הודעה מ{clients[notified_socket]}: {message}")

            for client_socket in clients:
                if client_socket != notified_socket:
                    try:
                        client_socket.send(message.encode('utf-8'))
                    except:
                        sockets_list.remove(client_socket)
                        del clients[client_socket]