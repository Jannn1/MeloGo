<?php
// Beállítások
$uploadDir = __DIR__ . '/uploads/'; // A feltöltések mappa elérési útja
if (!is_dir($uploadDir)) {
    mkdir($uploadDir, 0777, true);
}

header("Access-Control-Allow-Origin: *");
header("Access-Control-Allow-Methods: POST, GET, OPTIONS");
header("Access-Control-Allow-Headers: Content-Type, Authorization");
header("Content-Type: application/json");

// Kép átméretező függvény
function resizeImage($sourcePath, $destPath, $maxWidth, $maxHeight) {
    list($width, $height, $type) = getimagesize($sourcePath);
    $srcImage = null;

    switch ($type) {
        case IMAGETYPE_JPEG:
            $srcImage = imagecreatefromjpeg($sourcePath);
            break;
        case IMAGETYPE_PNG:
            $srcImage = imagecreatefrompng($sourcePath);
            break;
        case IMAGETYPE_GIF:
            $srcImage = imagecreatefromgif($sourcePath);
            break;
        default:
            return false;
    }

    if (!$srcImage) {
        return false;
    }

    $aspectRatio = $width / $height;
    if ($width > $maxWidth || $height > $maxHeight) {
        if ($width / $maxWidth > $height / $maxHeight) {
            $newWidth = $maxWidth;
            $newHeight = $maxWidth / $aspectRatio;
        } else {
            $newHeight = $maxHeight;
            $newWidth = $maxHeight * $aspectRatio;
        }
    } else {
        $newWidth = $width;
        $newHeight = $height;
    }

    $newImage = imagecreatetruecolor($newWidth, $newHeight);
    imagecopyresampled($newImage, $srcImage, 0, 0, 0, 0, $newWidth, $newHeight, $width, $height);

    $result = false;
    switch ($type) {
        case IMAGETYPE_JPEG:
            $result = imagejpeg($newImage, $destPath, 85);
            break;
        case IMAGETYPE_PNG:
            $result = imagepng($newImage, $destPath, 8);
            break;
        case IMAGETYPE_GIF:
            $result = imagegif($newImage, $destPath);
            break;
    }

    imagedestroy($srcImage);
    imagedestroy($newImage);

    return $result;
}

// Kérés feldolgozása
if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    if (isset($_FILES['image']) && $_FILES['image']['error'] === UPLOAD_ERR_OK) {
        $imageTmpPath = $_FILES['image']['tmp_name'];
        $imageExtension = strtolower(pathinfo($_FILES['image']['name'], PATHINFO_EXTENSION));
        $allowedExtensions = ['jpg', 'jpeg', 'png', 'gif'];

        if (in_array($imageExtension, $allowedExtensions)) {
            // Egyedi fájlnév generálása
            $newFileName = uniqid('image_', true) . '.' . $imageExtension;
            $destinationPath = $uploadDir . $newFileName;

            // Fájl mentése
            if (move_uploaded_file($imageTmpPath, $destinationPath)) {
                // Átméretezés
                $resizedFileName = 'resized_' . $newFileName;
                $resizedPath = $uploadDir . $resizedFileName;

                if (resizeImage($destinationPath, $resizedPath, 300, 300)) {
                    echo json_encode([
                        'success' => true,
                        'message' => 'Fájl feltöltve és átméretezve.',
                        'originalPath' => 'uploads/' . $newFileName,
                        'resizedPath' => 'uploads/' . $resizedFileName
                    ]);
                } else {
                    echo json_encode(['success' => false, 'message' => 'Hiba az átméretezés során.']);
                }
            } else {
                echo json_encode(['success' => false, 'message' => 'Hiba a fájl mentése során.']);
            }
        } else {
            echo json_encode(['success' => false, 'message' => 'Érvénytelen fájltípus. Engedélyezett típusok: ' . implode(', ', $allowedExtensions)]);
        }
    } else {
        echo json_encode(['success' => false, 'message' => 'Nincs fájl feltöltve vagy hibás a feltöltés.']);
    }
} else {
    echo json_encode(['success' => false, 'message' => 'Csak POST kérés engedélyezett.']);
}

?>