const API_BASE = "";

function showLoading(title, message) {

    document.getElementById("modalTitle").innerText =
        title;

    document.getElementById("modalMessage").innerText =
        message;

    document
        .getElementById("modalOverlay")
        .classList
        .remove("hidden");
}

function hideLoading() {

    document
        .getElementById("modalOverlay")
        .classList
        .add("hidden");
}

async function loadStatus() {

    try {

        const response =
            await fetch(`${API_BASE}/api/status`);

        const data =
            await response.json();

        document.getElementById("vpnStatus").innerText =
            data.vpnConnected
                ? "Connected"
                : "Disconnected";

        document.getElementById("routingMode").innerText =
            data.routingMode;

        document.getElementById("publicIp").innerText =
            data.publicIp;

        document.getElementById("cpu").innerText =
            `${data.cpuUsage}%`;

        document.getElementById("memory").innerText =
            `${data.memoryUsage}%`;

        document.getElementById("uptime").innerText =
            data.uptime;

        document.getElementById("lastUpdated").innerText =
            new Date(data.timestamp)
                .toLocaleString();

        if (data.traffic) {

            document.getElementById("todayTraffic").innerText =
                data.traffic.today.replace("Today: ", "");

            document.getElementById("monthTraffic").innerText =
                data.traffic.month.replace(/^.*?: /, "");

            document.getElementById("totalTraffic").innerText =
                data.traffic.total.replace(/^.*?: /, "");
        }
    }
    catch (error) {

        console.error(error);
    }
}

async function enableVpn() {

    showLoading(
        "Switching Route",
        "Routing traffic through VPN..."
    );

    try {

        const response =
            await fetch(
                `${API_BASE}/api/vpn/on`,
                {
                    method: "POST"
                });

        if (!response.ok) {
            throw new Error(
                "Failed to enable VPN routing");
        }

        await new Promise(
            resolve => setTimeout(resolve, 2000));

        await loadStatus();
    }
    catch (error) {

        alert(error.message);
    }
    finally {

        hideLoading();
    }
}

async function disableVpn() {

    showLoading(
        "Switching Route",
        "Routing traffic through WAN..."
    );

    try {

        const response =
            await fetch(
                `${API_BASE}/api/vpn/off`,
                {
                    method: "POST"
                });

        if (!response.ok) {
            throw new Error(
                "Failed to enable WAN routing");
        }

        await new Promise(
            resolve => setTimeout(resolve, 2000));

        await loadStatus();
    }
    catch (error) {

        alert(error.message);
    }
    finally {

        hideLoading();
    }
}

async function rebootWyse() {

    const confirmed =
        confirm(
            "Are you sure you want to reboot the Wyse?"
        );

    if (!confirmed) {
        return;
    }

    showLoading(
        "Rebooting Wyse",
        "The device is restarting..."
    );

    try {

        await fetch(
            `${API_BASE}/api/system/reboot`,
            {
                method: "POST"
            });
    }
    catch {

    }
}

loadStatus();

setInterval(
    loadStatus,
    10000);

if ("serviceWorker" in navigator) {

    navigator.serviceWorker
        .register("/service-worker.js")
        .then(() =>
            console.log(
                "Service Worker registered"))
        .catch(error =>
            console.error(
                "Service Worker error",
                error));
}