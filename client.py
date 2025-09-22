import socket
import threading
import tkinter as tk
from tkinter import scrolledtext, messagebox

HOST = '127.0.0.1'
PORT = 5555


class ChatClient:
    def __init__(self, root):
        self.root = root
        self.root.title("צאט ")
        self.root.geometry("500x400")

        self.client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

        try:
            self.client_socket.connect((HOST, PORT))
            print("התחברתי לשרת!")
        except Exception as e:
            messagebox.showerror("שגיאה", f"לא ניתן להתחבר לשרת: {e}")
            self.root.destroy()
            return

        self.create_widgets()

    def create_widgets(self):
        self.chat_area = scrolledtext.ScrolledText(self.root, wrap=tk.WORD, state='disabled')
        self.chat_area.pack(padx=10, pady=10, fill=tk.BOTH, expand=True)

        frame = tk.Frame(self.root)
        frame.pack(padx=10, pady=10, fill=tk.X)

        self.entry_message = tk.Entry(frame, font=("Arial", 12))
        self.entry_message.pack(side=tk.LEFT, fill=tk.X, expand=True)
        self.entry_message.bind("<Return>", self.send_message)

        self.btn_send = tk.Button(frame, text="שלח", command=self.send_message)
        self.btn_send.pack(side=tk.RIGHT, padx=(5, 0))

    def send_message(self, event=None):
        message = self.entry_message.get().strip()
        if message:
            try:
                self.client_socket.send(message.encode('utf-8'))
                self.entry_message.delete(0, tk.END)
            except Exception as e:
                messagebox.showerror("שגיאה", f"לא ניתן לשלוח הודעה: {e}")
                self.root.destroy()

    def receive_messages(self):
        while True:
            try:
                message = self.client_socket.recv(1024).decode('utf-8')
                if message:
                    self.root.after(0, self.update_chat, message)
            except Exception as e:
                print(f"שגיאה בקבלת הודעה: {e}")
                break

    def update_chat(self, message):
        self.chat_area.config(state='normal')
        self.chat_area.insert(tk.END, message + "\n")
        self.chat_area.config(state='disabled')
        self.chat_area.see(tk.END)

    def start(self):
        receive_thread = threading.Thread(target=self.receive_messages, daemon=True)
        receive_thread.start()
        self.root.protocol("WM_DELETE_WINDOW", self.on_closing)
        self.root.mainloop()

    def on_closing(self):
        try:
            self.client_socket.close()
        except:
            pass
        self.root.destroy()


if __name__ == "__main__":
    root = tk.Tk()
    client = ChatClient(root)
    client.start()