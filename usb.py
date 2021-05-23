import time
import asyncio
import websockets
import pywinusb.hid as hid

# NOTE: Requires patch in hid library to reduce the write timeout and hangs
# > result = winapi.WaitForSingleObject(overlapped_write.h_event, 10000 )
# in core.py file of `pywinusb.hid`
# Change 10 seconds to 250ms

async def handler(websocket, path):
    data = await websocket.recv()
    parts = str(data).split(":")
    set_color(int(parts[0]) + 1, int(parts[1]), int(parts[2]), int(parts[3]))


devices = hid.find_all_hid_devices()

r20d = None

for i in devices:
    # Janky way to get the device
    if i.product_name != "G102 LIGHTSYNC Gaming Mouse":
        continue

    elif "0004#" in i.device_path:
        r20d = i

r20d.open()


def set_color(z, r, g, b):
    def send(r, d):
        try:
            r.send(d)
            time.sleep(0.000001)
        except:
            print("Failed")
            pass

    r20r = r20d.find_output_reports()[0]

    # print(z, r, g, b)

    # Send 1st packet
    buffer = [0x00] * 20
    buffer[0] = 17
    buffer[1] = 255
    buffer[2] = 18
    buffer[3] = 26
    buffer[4] = z
    buffer[5] = r
    buffer[6] = g
    buffer[7] = b
    buffer[8] = 255
    send(r20r, buffer)
    r20r.send()

    # Send 2nd packet
    buffer = [0x00] * 20
    buffer[0] = 17
    buffer[1] = 255
    buffer[2] = 18
    buffer[3] = 122
    send(r20r, buffer)

    if r == 0 and g == 0 and b == 0:
        # Send 1st packet
        buffer = [0x00] * 20
        buffer[0] = 17
        buffer[1] = 255
        buffer[2] = 18
        buffer[3] = 26
        buffer[4] = z
        buffer[5] = r
        buffer[6] = g
        buffer[7] = b
        buffer[8] = 255
        send(r20r, buffer)
        r20r.send()

        # Send 2nd packet
        buffer = [0x00] * 20
        buffer[0] = 17
        buffer[1] = 255
        buffer[2] = 18
        buffer[3] = 122
        send(r20r, buffer)
        r20r.send()


set_color(1, 0, 0, 0)
set_color(2, 0, 0, 0)
set_color(3, 0, 0, 0)

start_server = websockets.serve(handler, "localhost", 8700)

asyncio.get_event_loop().run_until_complete(start_server)
asyncio.get_event_loop().run_forever()

