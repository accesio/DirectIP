#ifndef DIRECTIP_H
#define DIRECTIP_H

#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct {
    uint32_t ClientMsgId;
    uint32_t IdReference;
    int32_t Status;
} ConfirmationInfo;

int directip_send_mt(const char* host,
                     int port,
                     uint32_t client_msg_id,
                     const char* imei,
                     const uint8_t* payload,
                     int payload_len,
                     ConfirmationInfo* confirmation);

#ifdef __cplusplus
}
#endif

#endif // DIRECTIP_H
