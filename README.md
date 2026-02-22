Prerequisites for running the project locally:

- Godot v4.3
- .NET 8 SDK
- `server.exe` exported to the project folder with a custom feature tag: "server"

Uploading server to GCP:

- Export 'Export: Linux - Server' in Godot
- Zip `data_LowAge_linuxbsd_x86_64` and `server.x86_64` in `\export\linux`
- Upload the zipped file to [GCP VM](https://console.cloud.google.com/compute/instances?project=low-age-prototype) via SSH
- While uploading, remove previous files: `rm data_LowAge_linuxbsd_x86_64` and `rm server.x86_64`
- Unzip the file in GCP VM: `unzip linux.zip` (if command missing, run `sudo apt install unzip`)
- Run server with the following commands:
```
export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
chmod +x server.x86_64
./server.x86_64 --headless
```

Setup new server:
- Spin up a new Debian-based VM, make sure HTTP and HTTPS traffic is enabled
- Configure ingress and egress FW rules for port 3000
- Configure systemd in the VM `sudo nano /etc/systemd/system/lowageserver.service`:
```
[Unit]
Description=LowAge Godot Server
After=network.target

[Service]
User=simonas_letukas
WorkingDirectory=/home/simonas_letukas
ExecStart=/home/simonas_letukas/server.x86_64 --headless
Restart=always
RestartSec=5
LimitNOFILE=65535
Environment=DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1

[Install]
WantedBy=multi-user.target
```
Then: 
```
sudo systemctl enable lowageserver 
sudo systemctl start lowageserver
```
Diagnostics:
```
sudo systemctl status lowageserver
journalctl -u lowageserver -n 50 --no-pager
```