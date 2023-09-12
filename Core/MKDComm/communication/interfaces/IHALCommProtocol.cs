using mkdinfo.communication.protocol;
using System;
using System.Collections.Generic;

/*  
 * IHLHighLevelProtocol - Interface Hardware Layer - Hight Level Protocol
 * Interface para métodos de acesso padrões de objetos de protocolo de alto nível
 * 
 * Autor: Marcio Ken Iti Doi
 * Data: 30/04/2010
 * 
 * Todos os Direitos Reservados
 */

namespace mkdInfo.communication.interfaces
{
    public delegate void OnNewResponse(ResponseProtocolBase response);
    public delegate void OnError(Exception ex);

    public interface IHALCommProtocol
    {   
        void send(byte[] sendData);
        void start();
        void stop();

        OnNewResponse onNewResponse { get; set; }
        OnError onError { get; set; }
        IHALCommMedia media { get; set; }
        //public OnNewResponse onNewResponse = null;
        //public OnError onError = null;
    }
}
