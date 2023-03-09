async function createObjectUrl(fileStream) {
    const buffer = await fileStream.arrayBuffer();
    const blob = new Blob([buffer]);
    const url = URL.createObjectURL(blob);
    return url;
}

function revokeObjectUrl(url) {
    URL.revokeObjectURL(url);
}