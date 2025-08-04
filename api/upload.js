const axios = require('axios');

module.exports = async (req, res) => {
  if (req.method !== 'POST') {
    res.status(405).send('Method Not Allowed');
    return;
  }

  const surveyJson = JSON.stringify(req.body);
  const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
  const fileName = `survey_${timestamp}.json`;

  // TODO: Replace with your OneDrive access token
  const accessToken = process.env.ONEDRIVE_ACCESS_TOKEN;
  if (!accessToken) {
    res.status(500).send('OneDrive access token not set');
    return;
  }

  try {
    // Upload to OneDrive root folder (change path as needed)
    const uploadUrl = `https://graph.microsoft.com/v1.0/me/drive/root:/Surveys/${fileName}:/content`;
    const response = await axios.put(uploadUrl, surveyJson, {
      headers: {
        'Authorization': `Bearer ${accessToken}`,
        'Content-Type': 'application/json'
      }
    });
    res.status(200).send('Survey uploaded to OneDrive!');
  } catch (err) {
    res.status(500).send('Upload failed: ' + err.message);
  }
};
